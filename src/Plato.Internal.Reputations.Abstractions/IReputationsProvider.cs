using System.Collections.Generic;
using Plato.Internal.Models.Reputations;

namespace Plato.Internal.Reputations.Abstractions
{
    public interface IReputationsProvider<out TReputation> where TReputation : class, IReputation
    {
        IEnumerable<TReputation> GetReputations();
    }
}
