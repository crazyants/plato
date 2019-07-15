using System.Data;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.FederatedQueries;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Entities.Stores
{
    
    public class FeatureEntityCountStore  : IFeatureEntityCountStore
    {

        private readonly IFederatedQueryManager<FeatureEntityCount> _federatedQueryManager;
        private readonly IQueryAdapterManager<FeatureEntityCount> _queryAdapterManager;
        private readonly IFeatureEntityCountRepository _featureEntityCountRepository;
        private readonly IDbQueryConfiguration _dbQuery;

        public FeatureEntityCountStore(
            IFeatureEntityCountRepository featureEntityCountRepository,
            IFederatedQueryManager<FeatureEntityCount> federatedQueryManager,
            IQueryAdapterManager<FeatureEntityCount> queryAdapterManager,
            IDbQueryConfiguration dbQuery)
        {
            _featureEntityCountRepository = featureEntityCountRepository;
            _federatedQueryManager = federatedQueryManager;
            _queryAdapterManager = queryAdapterManager;
            _dbQuery = dbQuery;
        }
        
        public IQuery<FeatureEntityCount> QueryAsync()
        {
            return _dbQuery.ConfigureQuery(new FeatureEntityCountQuery<FeatureEntityCount>(this)
            {
                FederatedQueryManager = _federatedQueryManager,
                QueryAdapterManager = _queryAdapterManager
            });
        }

        public async Task<IPagedResults<FeatureEntityCount>> SelectAsync(IDbDataParameter[] dbParams)
        {
            return await _featureEntityCountRepository.SelectAsync(dbParams);
        }

    }

}
