using System.Collections.Generic;
using Plato.Entities.Reactions.Models;

namespace Plato.Entities.Reactions.Services
{
    public interface IReactionsProvider<out TReaction> where TReaction : class, IReaction
    {
        IEnumerable<TReaction> GetReactions();
    }

}
