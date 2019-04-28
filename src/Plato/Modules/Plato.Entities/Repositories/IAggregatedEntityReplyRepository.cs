using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Repositories.Metrics;

namespace Plato.Entities.Repositories
{
    public interface IAggregatedEntityReplyRepository : IAggregatedRepository
    {

        Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDateAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end,
            int featureId);

        Task<AggregatedResult<int>> SelectGroupedByIntAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end);

        Task<AggregatedResult<string>> SelectGroupedByFeatureAsync();

        Task<AggregatedResult<string>> SelectGroupedByFeatureAsync(int userId);

    }

}
