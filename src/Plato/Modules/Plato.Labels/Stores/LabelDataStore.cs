using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Repositories;

namespace Plato.Labels.Stores
{

    public class LabelDataStore : ILabelDataStore<LabelData>
    {
        
        private readonly ILabelDataRepository<LabelData> _labelDataRepository;
        private readonly ITypedModuleProvider _typedModuleProvider;
        private readonly ILogger<LabelDataStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public LabelDataStore(
            ILabelDataRepository<LabelData> labelDataRepository,
            ITypedModuleProvider typedModuleProvider,
            ILogger<LabelDataStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _labelDataRepository = labelDataRepository;
            _typedModuleProvider = typedModuleProvider;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }
        
        public async Task<LabelData> CreateAsync(LabelData model)
        {
            var result =  await _labelDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
        }

        public async Task<LabelData> UpdateAsync(LabelData model)
        {
            var result = await _labelDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
        }

        public async Task<bool> DeleteAsync(LabelData model)
        {
            var success = await _labelDataRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted eneity data with key '{0}' for entity id {1}",
                        model.Key, model.LabelId);
                }

                CancelTokens(model);

            }

            return success;
        }

        public async Task<LabelData> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _labelDataRepository.SelectByIdAsync(id));
        }

        public IQuery<LabelData> QueryAsync()
        {
            var query = new LabelDataQuery(this);
            return _dbQuery.ConfigureQuery<LabelData>(query); ;
        }

        public async Task<IPagedResults<LabelData>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _labelDataRepository.SelectAsync(dbParams));
        }

        public async Task<IEnumerable<LabelData>> GetByLabelIdAsync(int entityId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _labelDataRepository.SelectByLabelIdAsync(entityId));
        }

        public void CancelTokens(LabelData model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

    }

}
