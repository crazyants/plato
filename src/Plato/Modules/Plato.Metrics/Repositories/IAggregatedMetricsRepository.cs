using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Repositories.Metrics;

namespace Plato.Metrics.Repositories
{
    public interface IAggregatedMetricsRepository : IAggregatedRepository
    {

        Task<AggregatedResult<string>> SelectGroupedByFeature(DateTimeOffset start, DateTimeOffset end);

        Task<AggregatedResult<string>> SelectGroupedByRole(DateTimeOffset start, DateTimeOffset end);

    }
}
