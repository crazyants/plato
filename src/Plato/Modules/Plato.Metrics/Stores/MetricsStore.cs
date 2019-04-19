using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Metrics.Models;
using Plato.Metrics.Repositories;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Metrics.Stores
{

    public class MetricsStore : IMetricsStore<Metric>
    {

        private readonly IMetricsRepository<Metric> _metricRepository;
        private readonly ILogger<MetricsStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public MetricsStore(
            IMetricsRepository<Metric> metricRepository,
            ILogger<MetricsStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _metricRepository = metricRepository;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<Metric> CreateAsync(Metric model)
        {
            var result = await _metricRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<Metric> UpdateAsync(Metric model)
        {
            var result = await _metricRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<bool> DeleteAsync(Metric model)
        {
            var success = await _metricRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted metric with id {1}", model.Id);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<Metric> GetByIdAsync(int id)
        {
            return await _metricRepository.SelectByIdAsync(id);
        }

        public IQuery<Metric> QueryAsync()
        {
            var query = new MetricQuery(this);
            return _dbQuery.ConfigureQuery<Metric>(query); ;
        }

        public async Task<IPagedResults<Metric>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _metricRepository.SelectAsync(args));
        }
        
        #endregion
        
    }

}



