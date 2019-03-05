using System;
using System.Threading.Tasks;
using Plato.Entities.History.Models;
using Plato.Entities.History.Services;
using Plato.Entities.History.Stores;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Entities.History.Subscribers
{
    
    public class EntityReplySubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {

        private readonly IEntityReplyStore<EntityReply> _entityReplyStore;
        private readonly IEntityHistoryStore<EntityHistory> _entityHistoryStore;
        private readonly IEntityHistoryManager<EntityHistory> _entityHistoryManager;
        private readonly IBroker _broker;

        public EntityReplySubscriber(
            IBroker broker,
            IEntityHistoryStore<EntityHistory> entityHistoryStore,
            IEntityHistoryManager<EntityHistory> entityHistoryManager,
            IEntityReplyStore<EntityReply> entityReplyStore)
        {
            _broker = broker;
            _entityHistoryStore = entityHistoryStore;
            _entityHistoryManager = entityHistoryManager;
            _entityReplyStore = entityReplyStore;
        }

        #region "Implementation"

        public void Subscribe()
        {

            // Created
            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, async message => await EntityReplyCreated(message.What));

            // Updating
            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdating"
            }, async message => await EntityReplyUpdating(message.What));
            
            // Updated
            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdated"
            }, async message => await EntityReplyUpdated(message.What));

        }

        public void Unsubscribe()
        {

            // Created
            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, async message => await EntityReplyCreated(message.What));

            // Updating
            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdating"
            }, async message => await EntityReplyUpdating(message.What));
            
            // Updated
            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdated"
            }, async message => await EntityReplyUpdated(message.What));

        }
        
        public void Dispose()
        {
            Unsubscribe();
        }

        #endregion

        #region "Private Methods"

        async Task<TEntityReply> EntityReplyCreated(TEntityReply reply)
        {

            // Create entity reply history point
            await _entityHistoryManager.CreateAsync(new EntityHistory()
            {
                EntityId = reply.EntityId,
                EntityReplyId = reply.Id,
                Message = reply.Message,
                Html = reply.Html,
                CreatedUserId = reply.CreatedUserId,
                CreatedDate = reply.CreatedDate ?? DateTimeOffset.UtcNow
            });
            
            return reply;

        }

        async Task<TEntityReply> EntityReplyUpdating(TEntityReply reply)
        {

            // Get previous history points
            var previousHistories = await _entityHistoryStore.QueryAsync()
                .Take(1)
                .Select<EntityHistoryQueryParams>(q =>
                {
                    q.EntityId.Equals(reply.EntityId);
                    q.EntityReplyId.Equals(reply.Id);
                })
                .OrderBy("CreatedDate", OrderBy.Desc)
                .ToList();

            // Get the most recently added history point
            EntityHistory previousHistory = null;
            if (previousHistories?.Data != null)
            {
                previousHistory = previousHistories.Data[0];
            }

            // If we have previous history we don't need to add a starting point
            if (previousHistory != null)
            {
                return reply;
            }

            // Get existing reply before any changes
            var existingReply = await _entityReplyStore.GetByIdAsync(reply.Id);

            // We need an existing reply
            if (existingReply == null)
            {
                return reply;
            }

            // If we don't have any existing history points add our
            // existing reply (before updates) as the starting / original history point
            await _entityHistoryManager.CreateAsync(new EntityHistory()
            {
                EntityId = existingReply.EntityId,
                EntityReplyId = existingReply.Id,
                Message = existingReply.Message,
                Html = existingReply.Html,
                CreatedUserId = existingReply.CreatedUserId,
                CreatedDate = existingReply.CreatedDate
            });

            return reply;

        }
        
        async Task<TEntityReply> EntityReplyUpdated(TEntityReply reply)
        {
            
            // Get previous history points
            var previousHistories = await _entityHistoryStore.QueryAsync()
                .Take(1)
                .Select<EntityHistoryQueryParams>(q =>
                {
                    q.EntityId.Equals(reply.EntityId);
                    q.EntityReplyId.Equals(reply.Id);
                })
                .OrderBy("CreatedDate", OrderBy.Desc)
                .ToList();

            EntityHistory previousHistory = null;
            if (previousHistories?.Data != null)
            {
                previousHistory = previousHistories.Data[0];
            }

            // Ensure we actually have changes
            if (previousHistory != null)
            {
                // Don't save a history point if the Html has not changed
                if (reply.Html == previousHistory.Html)
                {
                    return reply;
                }
            }

            // Create entity reply history point
            await _entityHistoryManager.CreateAsync(new EntityHistory()
            {
                EntityId = reply.EntityId,
                EntityReplyId = reply.Id,
                Message = reply.Message,
                Html = reply.Html,
                CreatedUserId = reply.EditedUserId > 0 ? reply.EditedUserId : reply.ModifiedUserId,
                CreatedDate = reply.EditedDate ?? DateTimeOffset.UtcNow
            });
            
            return reply;

        }
        
        #endregion

    }

}
