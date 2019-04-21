using System.Threading.Tasks;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Repositories.Metrics;

namespace Plato.Entities.Repositories
{

    public interface IAggregatedEntityMetricsRepository : IAggregatedRepository
    {
        Task<AggregatedResult<string>> SelectGroupedByFeature();

        Task<AggregatedResult<string>> SelectGroupedByFeature(int userId);

    }

}
