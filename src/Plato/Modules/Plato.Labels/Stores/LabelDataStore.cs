using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Labels.Models;
using Plato.Labels.Repositories;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Labels.Stores
{

    public class LabelDataStore : ILabelDataStore<LabelData>
    {

        private readonly ICacheManager _cacheManager;
        private readonly ILabelDataRepository<LabelData> _labelDataRepository;
        private readonly ILogger<LabelDataStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ITypedModuleProvider _typedModuleProvider;
        
        public LabelDataStore(
            ICacheManager cacheManager,
            ILabelDataRepository<LabelData> labelDataRepository, 
            ILogger<LabelDataStore> logger,
            IDbQueryConfiguration dbQuery,
            ITypedModuleProvider typedModuleProvider)
        {
            _cacheManager = cacheManager;
            _labelDataRepository = labelDataRepository;
            _logger = logger;
            _dbQuery = dbQuery;
            _typedModuleProvider = typedModuleProvider;
        }
        
        public async Task<LabelData> CreateAsync(LabelData model)
        {
            var result =  await _labelDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<LabelData> UpdateAsync(LabelData model)
        {
            var result = await _labelDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
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

                _cacheManager.CancelTokens(this.GetType());
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

        public async Task<IPagedResults<LabelData>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _labelDataRepository.SelectAsync(args));
        }

        public async Task<IEnumerable<LabelData>> GetByLabelIdAsync(int entityId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _labelDataRepository.SelectByLabelIdAsync(entityId));
        }

    }

}
