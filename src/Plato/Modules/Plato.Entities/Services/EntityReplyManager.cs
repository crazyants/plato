using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Entities.Services
{
    public class EntityReplyManager : IEntityReplyManager<EntityReply>
    {

        public event EntityReplyEvents.Handler Creating;
        public event EntityReplyEvents.Handler Created;
        public event EntityReplyEvents.Handler Updating;
        public event EntityReplyEvents.Handler Updated;
        public event EntityReplyEvents.Handler Deleting;
        public event EntityReplyEvents.Handler Deleted;
     
        private readonly IBroker _broker;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IEntityReplyStore<EntityReply> _entityReplyStore;
        private readonly IContextFacade _contextFacade;

        public EntityReplyManager(
            IEntityReplyStore<EntityReply> entityReplyStore, 
            IBroker broker, IContextFacade contextFacade,
            IEntityStore<Entity> entityStore)
        {
            _entityReplyStore = entityReplyStore;
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _broker = broker;
        }

        public async Task<IActivityResult<EntityReply>> CreateAsync(EntityReply reply)
        {
            
            var result = new ActivityResult<EntityReply>();

          
            if (reply.EntityId <= 0)
            {
                return result.Failed(new EntityError($"{nameof(reply.EntityId)} must must be greater than zero"));
            }
            
            if (reply.Id > 0)
            {
                return result.Failed(new EntityError($"{nameof(reply.Id)} cannot be greater than zero when creating a reply"));
            }
            
            if (String.IsNullOrWhiteSpace(reply.Message))
            {
                return result.Failed(new EntityError($"{nameof(reply.Message)} is required"));
            }

            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return result.Failed(new EntityError($"An entity with the Id '{reply.EntityId}' could not be found"));
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                reply.CreatedUserId = user.Id;
                reply.ModifiedUserId = user.Id;
            }
           
            reply.CreatedDate = DateTime.UtcNow;
            reply.ModifiedDate = DateTime.UtcNow;

            // Parse Html and message abstract
            reply.Html = await ParseMarkdown(reply.Message);
            reply.Abstract = await ParseAbstract(reply.Message);
            
            // Raise creating event
            Creating?.Invoke(this, new EntityReplyEventArgs(entity, reply));

            // Publish EntityReplyCreating event
            await _broker.Pub<EntityReply>(this, new MessageOptions()
            {
                Key = "EntityReplyCreating"
            }, reply);

            var newReply = await _entityReplyStore.CreateAsync(reply);
            if (newReply != null)
            {
                // Raise created event
                Created?.Invoke(this, new EntityReplyEventArgs(entity, newReply));

                // Publish EntityReplyCreated event
                await _broker.Pub<EntityReply>(this, new MessageOptions()
                {
                    Key = "EntityReplyCreated"
                }, newReply);

                return result.Success(newReply);
            }

            return result.Failed(new EntityError("An unknown error occurred whilst attempting to create the reply"));

        }

        public async Task<IActivityResult<EntityReply>> UpdateAsync(EntityReply reply)
        {

            var result = new ActivityResult<EntityReply>();

            if (reply.Id <= 0)
            {
                return result.Failed(new EntityError($"{nameof(reply.Id)} must be a valid existing reply id"));
            }
            
            if (String.IsNullOrWhiteSpace(reply.Message))
            {
                return result.Failed(new EntityError($"{nameof(reply.Message)} is required"));
            }
            
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return result.Failed(new EntityError($"An entity with the Id '{reply.EntityId}' could not be found"));
            }
            
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                reply.ModifiedUserId = user.Id;
            }
            
            reply.ModifiedDate = DateTime.UtcNow;

            // Parse Html and message abstract
            reply.Html = await ParseMarkdown(reply.Message);
            reply.Abstract = await ParseAbstract(reply.Message);
            
            // Raise updating event
            Updating?.Invoke(this, new EntityReplyEventArgs(entity, reply));

            // Publish EntityReplyUpdating event
            await _broker.Pub<EntityReply>(this, new MessageOptions()
            {
                Key = "EntityReplyUpdating"
            }, reply);

            var updatedReply = await _entityReplyStore.UpdateAsync(reply);
            if (updatedReply != null)
            {
                // Raise Updated event
                Updated?.Invoke(this, new EntityReplyEventArgs(entity, updatedReply));

                // Publish EntityReplyUpdated event
                await _broker.Pub<EntityReply>(this, new MessageOptions()
                {
                    Key = "EntityReplyUpdated"
                }, updatedReply);

                return result.Success(updatedReply);
            }

            return result.Failed(new EntityError("An unknown error occurred whilst attempting to update the reply."));

        }

        public async Task<IActivityResult<EntityReply>> DeleteAsync(int id)
        {

            var result = new ActivityResult<EntityReply>();

            var reply = await _entityReplyStore.GetByIdAsync(id);
            if (reply == null)
            {
                return result.Failed(new EntityError($"An entity reply with the id of '{id}' could not be found"));
            }

            // Raise Deleting event
            Deleting?.Invoke(this, new EntityReplyEventArgs(null, reply));

            // Publish EntityReplyDeleting event
            await _broker.Pub<EntityReply>(this, new MessageOptions()
            {
                Key = "EntityReplyDeleting"
            }, reply);

            var success = await _entityReplyStore.DeleteAsync(reply);
            if (success)
            {

                // Raise Deleted event
                Deleted?.Invoke(this, new EntityReplyEventArgs(null, reply));

                // Publish EntityReplyDeleted event
                await _broker.Pub<EntityReply>(this, new MessageOptions()
                {
                    Key = "EntityReplyDeleted"
                }, reply);

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
