using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Entities.Follow.Repositories
{
    public interface IEntityFollowRepository<T> : IRepository<T> where T : class
    {

        Task<IEnumerable<T>> SelectEntityFollowsByEntityId(int entityId);

        Task<T> SelectEntityFollowsByUserIdAndEntityId(int userId, int entityId);

    }

}
