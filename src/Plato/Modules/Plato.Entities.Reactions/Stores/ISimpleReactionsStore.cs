using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Entities.Reactions.Models;

namespace Plato.Entities.Reactions.Stores
{
    public interface ISimpleReactionsStore
    {

        Task<IEnumerable<SimpleReaction>> GetSimpleReactionsAsync(int entityId, int entityReplyId);

    }

}
