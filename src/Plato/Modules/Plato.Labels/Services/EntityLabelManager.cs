using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Stores;

namespace Plato.Labels.Services
{
    
    public class EntityLabelManager : IEntityLabelManager<EntityLabel>
    {

        private readonly IEntityLabelStore<EntityLabel> _entityLabelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IBroker _broker;

        public EntityLabelManager(
            IEntityLabelStore<EntityLabel> entityLabelStore,
            IContextFacade contextFacade,
            IBroker broker)
        {
            _entityLabelStore = entityLabelStore;
            _broker = broker;
            _contextFacade = contextFacade;
        }

        public async Task<ICommandResult<EntityLabel>> CreateAsync(EntityLabel model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
             
            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }
            
            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }
            
            if (model.LabelId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.LabelId));
            }
            
            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            
            // Update
            model.CreatedUserId = user?.Id ?? 0;
            model.CreatedDate = DateTime.UtcNow;
            model.ModifiedUserId = user?.Id ?? 0;
            model.ModifiedDate = DateTime.UtcNow;

            // Invoke EntityLabelCreating subscriptions
            foreach (var handler in _broker.Pub<EntityLabel>(this, new MessageOptions()
            {
                Key = "EntityLabelCreating"
            }, model))
            {
                model = await handler.Invoke(new Message<EntityLabel>(model, this));
            }

            // Create result
            var result = new CommandResult<EntityLabel>();

            // Persist to database
            var newEntityLabel = await _entityLabelStore.CreateAsync(model);
            if (newEntityLabel != null)
            {

                // Invoke EntityLabelCreated subscriptions
                foreach (var handler in _broker.Pub<EntityLabel>(this, new MessageOptions()
                {
                    Key = "EntityLabelCreated"
                }, newEntityLabel))
                {
                    newEntityLabel = await handler.Invoke(new Message<EntityLabel>(newEntityLabel, this));
                }
                
                // Return success
                return result.Success(newEntityLabel);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an entity / label relatinship"));
            
        }

        public async Task<ICommandResult<EntityLabel>> UpdateAsync(EntityLabel model)
        {
            
            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            if (model.LabelId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.LabelId));
            }
            
            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();
         
            // Update
            model.ModifiedUserId = user?.Id ?? 0;
            model.ModifiedDate = DateTime.UtcNow;

            // Invoke EntityLabelUpdating subscriptions
            foreach (var handler in _broker.Pub<EntityLabel>(this, new MessageOptions()
            {
                Key = "EntityLabelUpdating"
            }, model))
            {
                model = await handler.Invoke(new Message<EntityLabel>(model, this));
            }
            
            // Create result
            var result = new CommandResult<EntityLabel>();

            // Persist to database
            var updatedEntityLabel = await _entityLabelStore.UpdateAsync(model);
            if (updatedEntityLabel != null)
            {

                // Invoke EntityLabelUpdated subscriptions
                foreach (var handler in _broker.Pub<EntityLabel>(this, new MessageOptions()
                {
                    Key = "EntityLabelUpdated"
                }, updatedEntityLabel))
                {
                    updatedEntityLabel = await handler.Invoke(new Message<EntityLabel>(updatedEntityLabel, this));
                }

                // Return success
                return result.Success(updatedEntityLabel);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an entity / label relatinship"));
            
        }

        public async Task<ICommandResult<EntityLabel>> DeleteAsync(EntityLabel model)
        {
            
            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Invoke EntityLabelDeleting subscriptions
            foreach (var handler in _broker.Pub<EntityLabel>(this, new MessageOptions()
            {
                Key = "EntityLabelDeleting"
            }, model))
            {
                model = await handler.Invoke(new Message<EntityLabel>(model, this));
            }

            var result = new CommandResult<EntityLabel>();
            if (await _entityLabelStore.DeleteAsync(model))
            {
             
                // Invoke EntityLabelDeleted subscriptions
                foreach (var handler in _broker.Pub<EntityLabel>(this, new MessageOptions()
                {
                    Key = "EntityLabelDeleted"
                }, model))
                {
                    model = await handler.Invoke(new Message<EntityLabel>(model, this));
                }

                // Return success
                return result.Success();

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the entity / label relationship"));


        }

    }

}
