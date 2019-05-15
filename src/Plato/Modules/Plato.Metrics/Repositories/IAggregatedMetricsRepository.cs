using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Repositories.Metrics;

namespace Plato.Metrics.Repositories
{
    public interface IAggregatedMetricsRepository : IAggregatedRepository
    {

        Task<AggregatedResult<string>> SelectGroupedByFeatureAsync(DateTimeOffset start, DateTimeOffset end);

        Task<AggregatedResult<string>> SelectGroupedByRoleAsync(DateTimeOffset start, DateTimeOffset end);

        Task<AggregatedResult<string>> SelectGroupedByTitleAsync(
            DateTimeOffset start,
            DateTimeOffset end,
            int limit = 20);

    }

}
