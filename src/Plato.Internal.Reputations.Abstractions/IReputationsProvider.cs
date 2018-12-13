using System.Collections.Generic;

namespace Plato.Internal.Reputations.Abstractions
{
    public interface IReputationsProvider<out TReputation> where TReputation : class, IReputation
    {
        IEnumerable<TReputation> GetReputations();
    }
}
