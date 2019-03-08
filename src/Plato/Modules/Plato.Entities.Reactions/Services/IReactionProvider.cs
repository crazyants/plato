using System.Collections.Generic;
using Plato.Reactions.Models;

namespace Plato.Reactions.Services
{
    public interface IReactionsProvider<out TReaction> where TReaction : class, IReaction
    {
        IEnumerable<TReaction> GetReactions();
    }

}
