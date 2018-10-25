using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;
using Plato.Reactions.Models;

namespace Plato.Reactions.Stores
{
    public interface IEntityReactionsStore<TModel> : IStore<TModel> where TModel : class
    {
        Task<IEnumerable<Reaction>> GetEntityReactionsAsync(int entityId);
    }

}
