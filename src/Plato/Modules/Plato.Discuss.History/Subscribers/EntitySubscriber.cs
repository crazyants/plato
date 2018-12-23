using System;
using System.Threading.Tasks;
using Plato.Entities.History.Models;
using Plato.Entities.History.Services;
using Plato.Entities.History.Stores;
using Plato.Entities.Models;
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
            //// Created
            //_broker.Sub<TEntity>(new MessageOptions()
            //{
            //    Key = "EntityCreated"
            //}, async message => await EntityCreated(message.What));

            // Updated
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
            
            // Create entity history entry
            var result = await _entityHistoryManager.CreateAsync(new EntityHistory()
            {
                EntityId = entity.Id,
                Message = entity.Message,
                Html = entity.Html,
                CreatedUserId = entity.ModifiedUserId,
                CreatedDate = DateTimeOffset.UtcNow
            });

            if (result.Succeeded)
            {

            }

            return entity;
            
        }

        async Task<TEntity> EntityUpdated(TEntity entity)
        {

            // Create entity history entry
            var result = await _entityHistoryManager.CreateAsync(new EntityHistory()
            {
                EntityId = entity.Id,
                Message = entity.Message,
                Html = entity.Html,
                CreatedUserId = entity.ModifiedUserId,
                CreatedDate = DateTimeOffset.UtcNow
            });

            if (result.Succeeded)
            {

            }

            return entity;

        }

        #endregion

    }

}
