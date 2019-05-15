using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Metrics;

namespace Plato.Internal.Repositories.Metrics
{
    public interface IAggregatedUserRepository : IAggregatedRepository
    {

        Task<AggregatedResult<string>> SelectUserMetricsAsync(DateTimeOffset start, DateTimeOffset end);
        
    }

}
