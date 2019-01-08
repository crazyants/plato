using System;
using System.Threading.Tasks;
using Plato.Entities.History.Models;
using Plato.Entities.History.Services;
using Plato.Entities.History.Stores;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Discuss.History.Subscribers
{
    
    public class EntityReplySubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {

        private readonly IEntityHistoryStore<EntityHistory> _entityHistoryStore;
        private readonly IEntityHistoryManager<EntityHistory> _entityHistoryManager;
        private readonly IBroker _broker;

        public EntityReplySubscriber(
            IBroker broker,
            IEntityHistoryStore<EntityHistory> entityHistoryStore,
            IEntityHistoryManager<EntityHistory> entityHistoryManager)
        {
            _broker = broker;
            _entityHistoryStore = entityHistoryStore;
            _entityHistoryManager = entityHistoryManager;
        }

        #region "Implementation"

        public void Subscribe()
        {
            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, async message => await EntityReplyCreated(message.What));

            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdated"
            }, async message => await EntityReplyUpdated(message.What));

        }

        public void Unsubscribe()
        {
            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, async message => await EntityReplyCreated(message.What));

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
                CreatedDate = DateTimeOffset.UtcNow
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
                CreatedUserId = reply.ModifiedUserId,
                CreatedDate = DateTimeOffset.UtcNow
            });
            
            return reply;

        }
        
        #endregion

    }

}
