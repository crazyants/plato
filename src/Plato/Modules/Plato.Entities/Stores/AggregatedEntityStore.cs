using System.Data;
using System.Threading.Tasks;
using Plato.Entities.Repositories;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    public interface IAggregatedFeatureEntitiesStore : IQueryableStore<AggregatedCount<string>>
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


        public IQuery<AggregatedCount<string>> QueryAsync()
        {
            var query = new AggregatedEntityQuery<AggregatedCount<string>>(this);
            return _dbQuery.ConfigureQuery<AggregatedCount<string>>(query); ;
        }

        public async Task<IPagedResults<AggregatedCount<string>>> SelectAsync(IDbDataParameter[] dbParams)
        {
            return await _aggregatedFeatureEntitiesRepository.SelectAsync(dbParams);
        }
    }
}
