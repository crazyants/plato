using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Repositories.Metrics;

namespace Plato.Entities.Metrics.Repositories
{
    public interface IAggregatedEntityMetricsRepository : IAggregatedRepository
    {
        Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDate(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end,
            int featureId);
    }
}
