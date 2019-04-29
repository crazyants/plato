using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Reputations.Abstractions
{
    public interface IReputationsManager<TReputation> where TReputation : class
    {
        Task<IEnumerable<TReputation>> GetReputationsAsync();

        Task<IDictionary<string, IEnumerable<TReputation>>> GetCategorizedReputationsAsync();

    }

}
