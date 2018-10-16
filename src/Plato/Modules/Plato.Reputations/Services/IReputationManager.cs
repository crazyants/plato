using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Reputations.Services
{
    public interface IReputationManager<TReputation> where TReputation : class
    {
        IEnumerable<TReputation> GetReputations();

        Task<IDictionary<string, IEnumerable<TReputation>>> GetCategorizedReputationsAsync();

    }

}
