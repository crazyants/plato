using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Repositories;
using Plato.Internal.Repositories.Metrics;

namespace Plato.Entities.Repositories
{

    public interface IAggregatedEntityRepository : IAggregatedRepository
    {

        Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDateAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end,
            int featureId);

        //Task<AggregatedResult<string>> SelectGroupedByFeatureAsync();

        //Task<AggregatedResult<string>> SelectGroupedByFeatureAsync(int userId);


    }

}
