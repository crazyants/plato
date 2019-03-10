using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Extensions;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Discuss.Subscribers
{

    /// <typeparam name="TEntity"></typeparam>
    public class TopicSubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IBroker _broker;

        private readonly IEntityStore<Topic> _entityStore;
        private readonly IUserReputationAwarder _reputationAwarder;

        public TopicSubscriber(
            IBroker broker, 
            IUserReputationAwarder reputationAwarder,
            IEntityStore<Topic> entityStore)
        {
            _broker = broker;
            _reputationAwarder = reputationAwarder;
            _entityStore = entityStore;
        }

        #region "Implementation"

        public void Subscribe()
        {
            // Created
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

            // Updating
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdating"
            }, async message => await EntityUpdating(message.What));

            // Updated
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated"
            }, async message => await EntityUpdated(message.What));

            // Deleted
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityDeleted"
            }, async message => await EntityDeleted(message.What));
        }

        public void Unsubscribe()
        {

            // Created
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));
            
            // Updating
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdating"
            }, async message => await EntityUpdating(message.What));

            // Updated
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated"
            }, async message => await EntityUpdated(message.What));

            // Deleted
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityDeleted"
            }, async message => await EntityDeleted(message.What));

        }
        
        #endregion

        #region "Private Methods"

        async Task<TEntity> EntityCreated(TEntity entity)
        {
            
            if (entity.IsPrivate)
            {
                return entity;
            }
            
            if (entity.IsDeleted)
            {
                return entity;
            }
            
            if (entity.IsSpam)
            {
                return entity;
            }

            // Award reputation
            await _reputationAwarder.AwardAsync(Reputations.NewTopic, entity.CreatedUserId);

            // Return
            return entity;

        }

        async Task<TEntity> EntityUpdating(TEntity entity)
        {
            
            // Get existing entity before any changes
            var existingEntity = await _entityStore.GetByIdAsync(entity.Id);

            // We need an existing entity
            if (existingEntity == null)
            {
                return entity;
            }

            // Entity has been hidden
            if (entity.IsHidden())
            {
                // If the existing entity was not hidden revoke reputation
                if (!existingEntity.IsHidden())
                {
                    await _reputationAwarder.RevokeAsync(Reputations.NewTopic, entity.CreatedUserId);
                }
            }
            else
            {
                // If the existing entity was hidden award reputation
                if (existingEntity.IsHidden())
                {
                    await _reputationAwarder.AwardAsync(Reputations.NewTopic, entity.CreatedUserId);
                }
            }
            
            // Return
            return entity;

        }

        Task<TEntity> EntityUpdated(TEntity entity)
        {
          
            // Return
            return Task.FromResult(entity);

        }

        async Task<TEntity> EntityDeleted(TEntity entity)
        {

            // Ensure we have a categoryId for the entity
            if (entity.CategoryId <= 0)
            {
                return entity;
            }

            // Revoke reputation
            await _reputationAwarder.RevokeAsync(Reputations.NewTopic, entity.CreatedUserId);

            // Return
            return entity;

        }
        
        #endregion

    }

}
