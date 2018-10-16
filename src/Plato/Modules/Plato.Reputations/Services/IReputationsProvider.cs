using System.Collections.Generic;
using Plato.Reputations.Models;

namespace Plato.Reputations.Services
{
    public interface IReputationsProvider<out TReputation> where TReputation : class, IReputation
    {

        IEnumerable<TReputation> GetReputations();

    }
}
