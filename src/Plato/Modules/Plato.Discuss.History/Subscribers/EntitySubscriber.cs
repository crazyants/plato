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

    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IEntityHistoryStore<EntityHistory> _entityHistoryStore;
        private readonly IEntityHistoryManager<EntityHistory> _entityHistoryManager;
        private readonly IBroker _broker;
      
        public EntitySubscriber(
            IBroker broker,
            IEntityHistoryManager<EntityHistory> entityHistoryManager, 
            IEntityHistoryStore<EntityHistory> entityHistoryStore)
        {
            _broker = broker;
            _entityHistoryManager = entityHistoryManager;
            _entityHistoryStore = entityHistoryStore;
        }

        #region "Implementation"

        public void Subscribe()
        {
   
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));
            
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated"
            }, async message => await EntityUpdated(message.What));
        }

        public void Unsubscribe()
        {

            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated"
            }, async message => await EntityUpdated(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<TEntity> EntityCreated(TEntity entity)
        {
            
            // Create entity history point
            await _entityHistoryManager.CreateAsync(new EntityHistory()
            {
                EntityId = entity.Id,
                Message = entity.Message,
                Html = entity.Html,
                CreatedUserId = entity.CreatedUserId,
                CreatedDate = entity.CreatedDate ?? DateTimeOffset.UtcNow
            });
            
            return entity;
            
        }

        async Task<TEntity> EntityUpdated(TEntity entity)
        {

            // Get previous history points
            var previousHistories = await _entityHistoryStore.QueryAsync()
                .Take(1)
                .Select<EntityHistoryQueryParams>(q =>
                {
                    q.EntityId.Equals(entity.Id);
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
                if (entity.Html == previousHistory.Html)
                {
                    return entity;
                }
            }

            // Create entity history point
            await _entityHistoryManager.CreateAsync(new EntityHistory()
            {
                EntityId = entity.Id,
                Message = entity.Message,
                Html = entity.Html,
                CreatedUserId = entity.EditedUserId > 0 ? entity.EditedUserId : entity.ModifiedUserId,
                CreatedDate = entity.EditedDate ?? DateTimeOffset.UtcNow
            });

            return entity;

        }

        #endregion

    }

}
