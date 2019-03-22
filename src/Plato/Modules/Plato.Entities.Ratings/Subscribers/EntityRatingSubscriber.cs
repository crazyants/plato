using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Ratings.Models;
using Plato.Entities.Ratings.Services;
using Plato.Entities.Ratings.Stores;
using Plato.Entities.Stores;
using Plato.Internal.Messaging.Abstractions;


namespace Plato.Entities.Ratings.Subscribers
{
    public class EntityRatingSubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IEntityRatingsAggregateStore _entityRatingsAggregateStore;
        private readonly IEntityStore<TEntity> _entityStore;
        private readonly IBroker _broker;
        
        public EntityRatingSubscriber(
            IEntityRatingsAggregateStore entityRatingsAggregateStore,
            IEntityStore<TEntity> entityStore,
            IBroker broker)
        {
            _entityRatingsAggregateStore = entityRatingsAggregateStore;
            _entityStore = entityStore;
            _broker = broker;
            
        }

        #region "Implementation"

        public void Subscribe()
        {

            // Created
            _broker.Sub<EntityRating>(new MessageOptions()
            {
                Key = "EntityRatingCreated"
            }, async message => await EntityRatingCreated(message.What));
            
            // Deleted
            _broker.Sub<EntityRating>(new MessageOptions()
            {
                Key = "EntityRatingDeleted"
            }, async message => await EntityRatingDeleted(message.What));

        }

        public void Unsubscribe()
        {

            // Created
            _broker.Unsub<EntityRating>(new MessageOptions()
            {
                Key = "EntityRatingCreated"
            }, async message => await EntityRatingCreated(message.What));

            // Deleted
            _broker.Unsub<EntityRating>(new MessageOptions()
            {
                Key = "EntityRatingDeleted"
            }, async message => await EntityRatingDeleted(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<EntityRating> EntityRatingCreated(EntityRating entityRating)
        {
            return await CalculateRatings(entityRating);
        }

        async Task<EntityRating> EntityRatingDeleted(EntityRating entityRating)
        {
            return await CalculateRatings(entityRating);
        }

        async Task<EntityRating> CalculateRatings(EntityRating entityRating)
        {


            if (entityRating == null)
            {
                throw new ArgumentNullException(nameof(entityRating));
            }

            if (entityRating.EntityId <= 0)
            {
                return entityRating;
            }

            // Get entity
            var entity = await _entityStore.GetByIdAsync(entityRating.EntityId);

            // No entity found no further work needed
            if (entity == null)
            {
                return entityRating;
            }
            
            var updatedRating = await _entityRatingsAggregateStore.SelectAggregateRating(entity.Id);

            // Update entity
            entity.TotalRatings = updatedRating?.TotalRatings ?? 0;
            entity.MeanRating = updatedRating?.MeanRating ?? 0;
            entity.DailyRatings = updatedRating?.DailyRatings ?? 0;

            // Persist label updates
            await _entityStore.UpdateAsync(entity);

            return entityRating;

        }
        
        #endregion

    }
}
