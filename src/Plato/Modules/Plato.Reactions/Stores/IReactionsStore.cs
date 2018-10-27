using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Reactions.Models;

namespace Plato.Reactions.Stores
{
    public interface IReactionsStore<TReaction> where TReaction : class, IReaction
    {
        Task<IEnumerable<TReaction>> GetEntityReactionsAsync(int entityId, int entityReplyId);
        
        Task<IEnumerable<GroupedReaction>> GetEntityReactionsGroupedAsync(int entityId, int entityReplyId);
        
    }

}
