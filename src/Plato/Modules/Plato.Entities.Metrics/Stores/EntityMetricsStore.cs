using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Metrics.Models;
using Plato.Entities.Metrics.Repositories;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Metrics.Stores
{

    public class EntityMetricsStore : IEntityMetricsStore<EntityMetric>
    {

        private readonly IEntityMetricsRepository<EntityMetric> _entityMetricRepository;
        private readonly ILogger<EntityMetricsStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public EntityMetricsStore(
            IEntityMetricsRepository<EntityMetric> entityMetricRepository,
            ILogger<EntityMetricsStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _entityMetricRepository = entityMetricRepository;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<EntityMetric> CreateAsync(EntityMetric model)
        {
            var result = await _entityMetricRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
        }

        public async Task<EntityMetric> UpdateAsync(EntityMetric model)
        {
            var result = await _entityMetricRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
        }

        public async Task<bool> DeleteAsync(EntityMetric model)
        {
            var success = await _entityMetricRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity metric with id {1}", model.Id);
                }

                CancelTokens(model);
            }

            return success;
        }

        public async Task<EntityMetric> GetByIdAsync(int id)
        {
            return await _entityMetricRepository.SelectByIdAsync(id);
        }

        public IQuery<EntityMetric> QueryAsync()
        {
            var query = new EntityMetricQuery(this);
            return _dbQuery.ConfigureQuery<EntityMetric>(query); ;
        }

        public async Task<IPagedResults<EntityMetric>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityMetricRepository.SelectAsync(dbParams));
        }
        
        #endregion

        public void CancelTokens(EntityMetric model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }
    }

}



