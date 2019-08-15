using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using Plato.Internal.Cache;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Categories.Stores
{

    public class CategoryDataStore : ICategoryDataStore<CategoryData>
    {

        public const string ById = "ById";
        public const string ByCategoryId = "ByCategoryId";

        private readonly ICacheManager _cacheManager;
        private readonly ICategoryDataRepository<CategoryData> _categoryDataRepository;
        private readonly ILogger<CategoryDataStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ITypedModuleProvider _typedModuleProvider;
        
        public CategoryDataStore(
            ICacheManager cacheManager,
            ICategoryDataRepository<CategoryData> categoryDataRepository, 
            ILogger<CategoryDataStore> logger,
            IDbQueryConfiguration dbQuery,
            ITypedModuleProvider typedModuleProvider)
        {
            _cacheManager = cacheManager;
            _categoryDataRepository = categoryDataRepository;
            _logger = logger;
            _dbQuery = dbQuery;
            _typedModuleProvider = typedModuleProvider;
        }
        
        public async Task<CategoryData> CreateAsync(CategoryData model)
        {
            var result =  await _categoryDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(model);
            }

            return result;
        }

        public async Task<CategoryData> UpdateAsync(CategoryData model)
        {
            var result = await _categoryDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(model);
            }

            return result;
        }

        public async Task<bool> DeleteAsync(CategoryData model)
        {
            var success = await _categoryDataRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted eneity data with key '{0}' for entity id {1}",
                        model.Key, model.CategoryId);
                }

                CancelTokens(model);
            }

            return success;
        }

        public async Task<CategoryData> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ById, id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _categoryDataRepository.SelectByIdAsync(id));
        }

        public IQuery<CategoryData> QueryAsync()
        {
            var query = new CategoryDataQuery(this);
            return _dbQuery.ConfigureQuery<CategoryData>(query); ;
        }

        public async Task<IPagedResults<CategoryData>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _categoryDataRepository.SelectAsync(dbParams));
        }

        public async Task<IEnumerable<CategoryData>> GetByCategoryIdAsync(int categoryId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByCategoryId, categoryId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _categoryDataRepository.SelectByCategoryIdAsync(categoryId));
        }

        public void CancelTokens(CategoryData model)
        {
            _cacheManager.CancelTokens(this.GetType());
            _cacheManager.CancelTokens(this.GetType(), ById, model.Id);
        }
    }

}
