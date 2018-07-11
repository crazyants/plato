using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Categories.Stores
{
    public class CategoryStore : ICategoryStore<Category>
    {

        private readonly ICategoryRepository<Category> _categoryRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<CategoryStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public CategoryStore(
            ICategoryRepository<Category> categoryRepository, 
            ICacheManager cacheManager,
            ILogger<CategoryStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _categoryRepository = categoryRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<Category> CreateAsync(Category model)
        {
            return await _categoryRepository.InsertUpdateAsync(model);
        }

        public async Task<Category> UpdateAsync(Category model)
        {
            return await _categoryRepository.InsertUpdateAsync(model);
        }

        public async Task<bool> DeleteAsync(Category model)
        {

            var success = await _categoryRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted category '{0}' with id {1}",
                        model.Name, model.Id);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;

        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _categoryRepository.SelectByIdAsync(id);
        }

        public IQuery<Category> QueryAsync()
        {
            var query = new CategoryQuery(this);
            return _dbQuery.ConfigureQuery<Category>(query); ;
        }

        public async Task<IPagedResults<Category>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entities for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _categoryRepository.SelectAsync(args);
              
            });
        }

        #endregion


    }
}
