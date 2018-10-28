using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Reactions.Models;

namespace Plato.Reactions.Stores
{
    public interface IReactionsEntryStore<TReaction> where TReaction : class, IReactionEntry
    {
        Task<IDictionary<string, ReactionEntryGrouped>> GetReactionsAsync(int entityId, int entityReplyId);
        
        Task<IEnumerable<GroupedReaction>> GetReactionsGroupedAsync(int entityId, int entityReplyId);
        
    }

}
