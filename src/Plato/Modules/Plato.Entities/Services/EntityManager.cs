using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Entities.Services
{
    
    public class EntityManager : IEntityManager<Entity>
    {

        public event EntityEvents.Handler Creating;
        public event EntityEvents.Handler Created;
        public event EntityEvents.Handler Updating;
        public event EntityEvents.Handler Updated;
        public event EntityEvents.Handler Deleting;
        public event EntityEvents.Handler Deleted;
   
        #region "Constructor"

        private readonly IBroker _broker;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IContextFacade _contextFacade;

        public EntityManager(
            IEntityStore<Entity> entityStore,
            IBroker broker, 
            IContextFacade contextFacade)
        {
            _entityStore = entityStore;
            _broker = broker;
            _contextFacade = contextFacade;
        }

        #endregion

        #region "Implementation"

        public async Task<IEntityResult> CreateAsync(Entity model)
        {

            var result = new EntityResult();
            
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            var feature = await _contextFacade.GetCurrentFeatureAsync();

            // Set entities featureId based on current feature
            if (feature != null)
            {
                model.FeatureId = feature.Id;
            }
            
            if (user != null)
            {
                model.CreatedUserId = user.Id;
                model.ModifiedUserId = user.Id;
            }

            model.CreatedDate = DateTime.UtcNow;
            model.ModifiedDate = DateTime.UtcNow;

            // Validate
            if (model.Id > 0)
            {
                return result.Failed(new EntityError($"{nameof(model.Id)} cannot be greater than zero when creating an entity"));
            }

            if (model.FeatureId == 0)
            {
                return result.Failed(new EntityError($"{nameof(model.FeatureId)} must be greater than zero when creating an entity"));
            }

            if (String.IsNullOrWhiteSpace(model.Title))
            {
                return result.Failed(new EntityError($"{nameof(model.Title)} is required"));
            }

            if (String.IsNullOrWhiteSpace(model.Message))
            {
                return result.Failed(new EntityError($"{nameof(model.Message)} is required"));
            }

            // Parse Html and message abstract
            model.Html = await ParseMarkdown(model.Message);
            model.Abstract = await ParseAbstract(model.Message);
            
            // Raise creating event
            Creating?.Invoke(this, new EntityEventArgs()
            {
                Entity = model
            });
            
            var entity = await _entityStore.CreateAsync(model);
            if (entity != null)
            {
                // Raise created event
                Created?.Invoke(this, new EntityEventArgs()
                {
                    Entity = entity
                });
                // Return success
                return result.Success(entity);
            }

            return result.Failed(new EntityError("An unknown error occurred whilst attempting to create an eneity"));

        }

        public async Task<IEntityResult> UpdateAsync(Entity model)
        {

            var result = new EntityResult();

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            if (user != null)
            {
                model.ModifiedUserId = user.Id;
            }

            model.ModifiedDate = DateTime.UtcNow;

            // Validate
            if (model.Id <= 0)
            {
                return result.Failed(new EntityError($"{nameof(model.Id)} must be a valid existing entity id"));
            }

            if (String.IsNullOrWhiteSpace(model.Title))
            {
                return result.Failed(new EntityError($"{nameof(model.Title)} is required"));
            }

            if (String.IsNullOrWhiteSpace(model.Message))
            {
                return result.Failed(new EntityError($"{nameof(model.Message)} is required"));
            }

            // Parse Html and message abstract
            model.Html = await ParseMarkdown(model.Message);
            model.Abstract = await ParseAbstract(model.Message);

            // Raise updating event
            Updating?.Invoke(this, new EntityEventArgs()
            {
                Entity = model
            });
            
            var entity = await _entityStore.UpdateAsync(model);
            if (entity != null)
            {
                Updated?.Invoke(this, new EntityEventArgs()
                {
                    Entity = entity
                });
                return result.Success(entity);
            }

            return result.Failed(new EntityError("An unknown error occurred whilst attempting to create an eneity."));

        }

        public async Task<IEntityResult> DeleteAsync(int id)
        {

            var result = new EntityResult();

            var entity = await _entityStore.GetByIdAsync(id);
            if (entity == null)
            {
                return result.Failed(new EntityError($"An entity is the id {id} could not be found"));
            }

            Deleting?.Invoke(this, new EntityEventArgs()
            {
                Entity = entity
            });

            var success = await _entityStore.DeleteAsync(entity);
            if (success)
            {
                Deleted?.Invoke(this, new EntityEventArgs()
                {
                    Success = true,
                    Entity = entity
                });
                return result.Success(entity);
            }

            return result.Failed(new EntityError("An unknown error occurred whilst attempting to create an eneity."));

        }

        #endregion

        #region "Private Methods"

        private async Task<string> ParseMarkdown(string message)
        {

            foreach (var handler in await _broker.Pub<string>(this, new MessageOptions()
            {
                Key = "ParseMarkdown"
            }, message))
            {
                return await handler.Invoke(new Message<string>(message, this));
            }

            return message;

        }

        private async Task<string> ParseAbstract(string message)
        {

            foreach (var handler in await _broker.Pub<string>(this, new MessageOptions()
            {
                Key = "ParseAbstract"
            }, message))
            {
                return await handler.Invoke(new Message<string>(message, this));
            }

            return message.StripHtml().TrimToAround(500);

        }
        
        #endregion

    }

}
