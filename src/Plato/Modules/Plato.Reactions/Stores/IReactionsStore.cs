using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Reactions.Models;

namespace Plato.Reactions.Stores
{
    public interface IReactionsStore<TReaction> where TReaction : class, IReaction
    {
        Task<IEnumerable<TReaction>> GetEntityReactionsAsync(int entityId);
        
        Task<IDictionary<string, IList<IReaction>>> GetEntityReactionsGroupedByEmojiAsync(int entityId);

    }

}
