using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Ratings.Stores
{
    public interface IEntityRatingsStore<TModel> : IStore2<TModel> where TModel : class
    {
     
        Task<IEnumerable<TModel>> SelectEntityRatingsByEntityId(int entityId);

        Task<IEnumerable<TModel>> SelectEntityRatingsByUserIdAndEntityId(int userId, int entityId);

    }

}
