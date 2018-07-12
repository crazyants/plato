using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Categories.Stores
{

    public interface ICategoryDataStore<T> : IStore<T> where T : class
    {

        Task<IEnumerable<T>> GetByCategoryIdAsync(int entityId);

    }

    public class CategoryDataStore : ICategoryDataStore<CategoryData>
    {

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
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<CategoryData> UpdateAsync(CategoryData model)
        {
            var result = await _categoryDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
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

                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<CategoryData> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _categoryDataRepository.SelectByIdAsync(id));
        }

        public IQuery<CategoryData> QueryAsync()
        {
            var query = new CategoryDataQuery(this);
            return _dbQuery.ConfigureQuery<CategoryData>(query); ;
        }

        public async Task<IPagedResults<CategoryData>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _categoryDataRepository.SelectAsync(args));
        }

        public async Task<IEnumerable<CategoryData>> GetByCategoryIdAsync(int entityId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _categoryDataRepository.SelectByCategoryIdAsync(entityId));
        }

    }

}
