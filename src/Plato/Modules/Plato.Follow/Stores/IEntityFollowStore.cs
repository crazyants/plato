using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Follow.Stores
{
    public interface IEntityFollowStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectEntityFollowsByEntityId(int entityId);

        Task<TModel> SelectEntityFollowByUserIdAndEntityId(int userId, int entityId);

    }


}
