using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Metrics;

namespace Plato.Internal.Repositories.Metrics
{
    public interface IAggregatedRepository
    {

        Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDate(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end);

    }


}
