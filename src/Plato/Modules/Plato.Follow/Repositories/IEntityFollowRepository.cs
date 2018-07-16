using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Follow.Repositories
{
    public interface IEntityFollowRepository<TModel> : IRepository<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectEntityFollowsByEntityId(int entityId);

        Task<TModel> SelectEntityFollowByUserIdAndEntityId(int userId, int entityId);

    }

}
