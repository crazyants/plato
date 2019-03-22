using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Entities.Ratings.Repositories
{
    public interface IEntityRatingsRepository<TModel> : IRepository<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectEntityRatingsByEntityId(int entityId);

        Task<IEnumerable<TModel>> SelectEntityRatingsByUserIdAndEntityId(int userId, int entityId);
        
    }

}