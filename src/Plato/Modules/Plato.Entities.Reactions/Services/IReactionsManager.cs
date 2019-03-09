using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Entities.Reactions.Services
{
    public interface IReactionsManager<TReaction> where TReaction : class
    {
        IEnumerable<TReaction> GetReactions();

        Task<IDictionary<string, IEnumerable<TReaction>>> GetCategorizedReactionsAsync();

    }

}
