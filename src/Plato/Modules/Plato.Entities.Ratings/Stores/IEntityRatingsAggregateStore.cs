using System.Threading.Tasks;
using Plato.Entities.Ratings.Models;

namespace Plato.Entities.Ratings.Stores
{
    public interface IEntityRatingsAggregateStore
    {

        Task<AggregateRating> SelectAggregateRating(int entityId);

        Task<AggregateRating> SelectAggregateRating(int entityId, int replyId);

    }
    
}
