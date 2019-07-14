using System.Data;
using System.Threading.Tasks;
using Plato.Entities.Repositories;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    public interface IAggregatedEntityStore : IQueryableStore<AggregatedCount<string>>
    {

    }

    public class AggregatedEntityStore  : IAggregatedEntityStore
    {

        private readonly IAggregatedEntityRepository _aggregatedEntityRepository;
        private readonly IDbQueryConfiguration _dbQuery;

        public AggregatedEntityStore(
            IAggregatedEntityRepository aggregatedEntityRepository,
            IDbQueryConfiguration dbQuery)
        {
            _aggregatedEntityRepository = aggregatedEntityRepository;
            _dbQuery = dbQuery;
        }


        public IQuery<AggregatedCount<string>> QueryAsync()
        {
            var query = new AggregatedEntityQuery<AggregatedCount<string>>(this);
            return _dbQuery.ConfigureQuery<AggregatedCount<string>>(query); ;
        }

        public async Task<IPagedResults<AggregatedCount<string>>> SelectAsync(IDbDataParameter[] dbParams)
        {
            return await _aggregatedEntityRepository.SelectAsync(dbParams);
        }
    }
}
