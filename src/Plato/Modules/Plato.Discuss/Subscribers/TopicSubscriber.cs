using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Extensions;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Entities.Stores;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Discuss.Subscribers
{

    /// <typeparam name="TEntity"></typeparam>
    public class TopicSubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IEntityRepository<TEntity> _entityRepository;
        private readonly IUserReputationAwarder _reputationAwarder;
        private readonly IBroker _broker;

        public TopicSubscriber(
            IUserReputationAwarder reputationAwarder,
            IEntityRepository<TEntity> entityRepository,
            IBroker broker)
        {
            _reputationAwarder = reputationAwarder;
            _entityRepository = entityRepository;
            _broker = broker;
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

        }

        #endregion

        #region "Private Methods"

        async Task<TEntity> EntityCreated(TEntity entity)
        {

            if (entity.IsHidden())
            {
                return entity;
            }

            // Award reputation
            await _reputationAwarder.AwardAsync(Reputations.NewTopic, entity.CreatedUserId, "Topic posted");

            // Return
            return entity;

        }

        async Task<TEntity> EntityUpdating(TEntity entity)
        {

            // Get existing entity before any changes
            var existingEntity = await _entityRepository.SelectByIdAsync(entity.Id);

            // We need an existing entity
            if (existingEntity == null)
            {
                return entity;
            }

            // Entity has been hidden
            if (entity.IsHidden())
            {
                // If the existing entity was not already hidden revoke reputation
                if (!existingEntity.IsHidden())
                {
                    await _reputationAwarder.RevokeAsync(Reputations.NewTopic, entity.CreatedUserId, "Topic deleted or hidden");
                }
            }
            else
            {
                // If the existing entity was hidden award reputation
                if (existingEntity.IsHidden())
                {
                    await _reputationAwarder.AwardAsync(Reputations.NewTopic, entity.CreatedUserId, "Topic approved or made visible");
                }
            }

            // Return
            return entity;

        }

        #endregion

    }

}
