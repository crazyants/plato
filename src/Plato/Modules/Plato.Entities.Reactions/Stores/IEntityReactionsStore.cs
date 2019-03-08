using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Reactions.Stores
{
    public interface IEntityReactionsStore<TModel> : IStore<TModel> where TModel : class
    {
     
        Task<IEnumerable<TModel>> SelectEntityReacotinsByEntityId(int entityId);

        Task<IEnumerable<TModel>> SelectEntityReactionsByUserIdAndEntityId(int userId, int entityId);

    }

}
