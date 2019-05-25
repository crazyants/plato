using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Reactions.Stores
{
    public interface IEntityReactionsStore<TModel> : IStore2<TModel> where TModel : class
    {
     
        Task<IEnumerable<TModel>> SelectEntityReactionsByEntityId(int entityId);

        Task<IEnumerable<TModel>> SelectEntityReactionsByUserIdAndEntityId(int userId, int entityId);

    }

}
