using System.Data;
using System.Threading.Tasks;
using Plato.Entities.Repositories;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    public interface IAggregatedFeatureEntitiesStore : IQueryableStore<AggregatedResult<string>>
    {

    }

    public class AggregatedFeatureEntitiesStore  : IAggregatedFeatureEntitiesStore
    {

        private readonly IAggregatedFeatureEntitiesRepository _aggregatedFeatureEntitiesRepository;
        private readonly IDbQueryConfiguration _dbQuery;

        public AggregatedFeatureEntitiesStore(
            IAggregatedFeatureEntitiesRepository aggregatedFeatureEntitiesRepository,
            IDbQueryConfiguration dbQuery)
        {
            _aggregatedFeatureEntitiesRepository = aggregatedFeatureEntitiesRepository;
            _dbQuery = dbQuery;
        }


        public IQuery<AggregatedResult<string>> QueryAsync()
        {
            var query = new AggregatedEntityQuery<AggregatedResult<string>>(this);
            return _dbQuery.ConfigureQuery<AggregatedResult<string>>(query); ;
        }

        public async Task<IPagedResults<AggregatedResult<string>>> SelectAsync(IDbDataParameter[] dbParams)
        {
            return await _aggregatedFeatureEntitiesRepository.SelectAsync(dbParams);
        }
    }
}
