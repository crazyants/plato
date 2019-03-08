using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Reactions.Models;

namespace Plato.Reactions.Stores
{
    public interface ISimpleReactionsStore
    {

        Task<IEnumerable<SimpleReaction>> GetSimpleReactionsAsync(int entityId, int entityReplyId);

    }

}
