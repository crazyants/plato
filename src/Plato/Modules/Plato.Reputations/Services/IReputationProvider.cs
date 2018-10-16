using System.Collections.Generic;
using Plato.Reputations.Models;

namespace Plato.Reputations.Services
{
    public interface IReputationProvider<out TReputation> where TReputation : class, IReputation
    {

        IEnumerable<TReputation> GetReputations();

    }
}
