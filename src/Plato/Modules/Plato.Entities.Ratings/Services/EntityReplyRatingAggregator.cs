using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Ratings.Stores;
using Plato.Entities.Stores;

namespace Plato.Entities.Ratings.Services
{
  
    public class EntityReplyRatingAggregator<TEntityReply> : IEntityReplyRatingAggregator<TEntityReply> where TEntityReply : class, IEntityReply
    {

        private readonly IEntityRatingsAggregateStore _entityRatingsAggregateStore;
        private readonly IEntityReplyStore<TEntityReply> _entityStore;

        public EntityReplyRatingAggregator(
            IEntityRatingsAggregateStore entityRatingsAggregateStore,
            IEntityReplyStore<TEntityReply> entityStore)
        {
            _entityRatingsAggregateStore = entityRatingsAggregateStore;
            _entityStore = entityStore;
        }

        public async Task<TEntityReply> UpdateAsync(TEntityReply reply)
        {

            // Get aggregate rating
            var aggregateRating = await _entityRatingsAggregateStore.SelectAggregateRating(reply.EntityId, reply.Id);

            // Update entity
            reply.TotalRatings = aggregateRating?.TotalRatings ?? 0;
            reply.SummedRating = aggregateRating?.SummedRating ?? 0;
            reply.MeanRating = aggregateRating?.MeanRating ?? 0;
            
            // Update and return updated entity
            return await _entityStore.UpdateAsync(reply);

        }

    }

}
