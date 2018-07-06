using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Entities.Services
{
    public class EntityReplyManager : IEntityManager<EntityReply>
    {

        public event EntityEvents.EntityEventHandler Creating;
        public event EntityEvents.EntityEventHandler Created;
        public event EntityEvents.EntityEventHandler Updating;
        public event EntityEvents.EntityEventHandler Updated;
        public event EntityEvents.EntityEventHandler Deleting;
        public event EntityEvents.EntityEventHandler Deleted;
     
        private readonly IBroker _broker;
        private readonly IEntityReplyStore<EntityReply> _entityReplyStore;
        private readonly IContextFacade _contextFacade;

        public EntityReplyManager(
            IEntityReplyStore<EntityReply> entityReplyStore, 
            IBroker broker, IContextFacade contextFacade)
        {
            _entityReplyStore = entityReplyStore;
            _contextFacade = contextFacade;
            _broker = broker;
        }

        public async Task<IEntityResult> CreateAsync(EntityReply model)
        {
            
            var result = new EntityResult();

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            
            if (model.EntityId <= 0)
            {
                return result.Failed(new EntityError($"{nameof(model.EntityId)} must must be greater than zero"));
            }
            
            if (model.Id > 0)
            {
                return result.Failed(new EntityError($"{nameof(model.Id)} cannot be greater than zero when creating a reply"));
            }
            
            if (String.IsNullOrWhiteSpace(model.Message))
            {
                return result.Failed(new EntityError($"{nameof(model.Message)} is required"));
            }

            model.CreatedUserId = user.Id;
            model.CreatedDate = DateTime.UtcNow;
            model.ModifiedUserId = user.Id;
            model.ModifiedDate = DateTime.UtcNow;

            // Parse Html and message abstract
            model.Html = await ParseMarkdown(model.Message);
            model.Abstract = await ParseAbstract(model.Message);
            
            // Raise creating event
            Creating?.Invoke(this, new EntityManagerEventArgs()
            {
                Model = model
            });
            
            var reply = await _entityReplyStore.CreateAsync(model);
            if (reply != null)
            {
                // Raise created event
                Created?.Invoke(this, new EntityManagerEventArgs()
                {
                    Model = reply
                });
                return result.Success(reply);
            }

            return result.Failed(new EntityError("An unknown error occurred whilst attempting to create the reply"));

        }

        public async Task<IEntityResult> UpdateAsync(EntityReply model)
        {

            var result = new EntityResult();

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            if (model.Id <= 0)
            {
                return result.Failed(new EntityError($"{nameof(model.Id)} must be a valid existing reply id"));
            }
            
            if (String.IsNullOrWhiteSpace(model.Message))
            {
                return result.Failed(new EntityError($"{nameof(model.Message)} is required"));
            }

            model.ModifiedUserId = user.Id;
            model.ModifiedDate = DateTime.UtcNow;

            // Parse Html and message abstract
            model.Html = await ParseMarkdown(model.Message);
            model.Abstract = await ParseAbstract(model.Message);
            
            // Raise updating event
            Updating?.Invoke(this, new EntityManagerEventArgs()
            {
                Model = model
            });


            var reply = await _entityReplyStore.UpdateAsync(model);
            if (reply != null)
            {
                Updated?.Invoke(this, new EntityManagerEventArgs()
                {
                    Model = reply
                });
                return result.Success(reply);
            }

            return result.Failed(new EntityError("An unknown error occurred whilst attempting to update the reply."));

        }

        public async Task<IEntityResult> DeleteAsync(int id)
        {

            var result = new EntityResult();

            var reply = await _entityReplyStore.GetByIdAsync(id);
            if (reply == null)
            {
                return result.Failed(new EntityError($"An entity reply with the id of '{id}' could not be found"));
            }

            Deleting?.Invoke(this, new EntityManagerEventArgs()
            {
                Model = reply
            });
            
            var success = await _entityReplyStore.DeleteAsync(reply);
            Deleted?.Invoke(this, new EntityManagerEventArgs()
            {
                Success = success,
                Model = reply
            });

            if (success)
            {
                return result.Success(reply);
            }

            return result.Failed(new EntityError("An unknown error occurred whilst attempting to delete the reply."));

        }
        
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
