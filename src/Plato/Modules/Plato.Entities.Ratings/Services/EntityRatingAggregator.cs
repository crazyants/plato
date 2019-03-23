using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Ratings.Stores;
using Plato.Entities.Stores;

namespace Plato.Entities.Ratings.Services
{
 
    public class EntityRatingAggregator<TEntity> : IEntityRatingAggregator<TEntity> where TEntity : class, IEntity
    {

        private readonly IEntityRatingsAggregateStore _entityRatingsAggregateStore;
        private readonly IEntityStore<TEntity> _entityStore;

        public EntityRatingAggregator(
            IEntityRatingsAggregateStore entityRatingsAggregateStore,
            IEntityStore<TEntity> entityStore)
        {
            _entityRatingsAggregateStore = entityRatingsAggregateStore;
            _entityStore = entityStore;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            
            // Get aggregate rating
            var aggregateRating = await _entityRatingsAggregateStore.SelectAggregateRating(entity.Id);

            // Update entity
            entity.TotalRatings = aggregateRating?.TotalRatings ?? 0;
            entity.SummedRating = aggregateRating?.SummedRating ?? 0;
            entity.MeanRating = aggregateRating?.MeanRating ?? 0;
            entity.DailyRatings = aggregateRating?.DailyRatings ?? 0;
            
            // Update and return updated entity
            return await _entityStore.UpdateAsync(entity);

        }

    }

}
