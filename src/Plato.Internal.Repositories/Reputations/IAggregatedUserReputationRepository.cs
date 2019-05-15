using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Repositories.Metrics;

namespace Plato.Internal.Repositories.Reputations
{
    public interface IAggregatedUserReputationRepository : IAggregatedRepository
    {

        Task<AggregatedResult<int>> SelectSummedByIntAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end);

        Task<AggregatedResult<int>> SelectSummedByIntAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end,
            int featureId);

        Task<AggregatedResult<string>> SelectGroupedByFeature(DateTimeOffset start, DateTimeOffset end);
        Task<AggregatedResult<DateTimeOffset>> SelectGroupedByNameAsync(
            string reputationName,
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end);

    }

}
