using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;

namespace Plato.Categories.Stores
{

    public class CategoryRoleStore : ICategoryRoleStore<CategoryRole>
    {

        private readonly ICategoryRoleRepository<CategoryRole> _categoryRoleRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<CategoryRoleStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public CategoryRoleStore(
            ICategoryRoleRepository<CategoryRole> categoryRoleRepository,
            ICacheManager cacheManager,
            ILogger<CategoryRoleStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _categoryRoleRepository = categoryRoleRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<CategoryRole> CreateAsync(CategoryRole model)
        {
            return await _categoryRoleRepository.InsertUpdateAsync(model);
        }

        public async Task<CategoryRole> UpdateAsync(CategoryRole model)
        {
            return await _categoryRoleRepository.InsertUpdateAsync(model);
        }

        public async Task<bool> DeleteAsync(CategoryRole model)
        {

            var success = await _categoryRoleRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted category role for category '{0}' with id {1}",
                        model.CategoryId, model.Id);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;

        }

        public async Task<CategoryRole> GetByIdAsync(int id)
        {
            return await _categoryRoleRepository.SelectByIdAsync(id);
        }

        public IQuery<CategoryRole> QueryAsync()
        {
            var query = new CategoryRoleQuery(this);
            return _dbQuery.ConfigureQuery<CategoryRole>(query); ;
        }

        public async Task<IPagedResults<CategoryRole>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entities for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _categoryRoleRepository.SelectAsync(args);

            });
        }

        #endregion
        
        public async Task<IEnumerable<CategoryRole>> GetByCategoryIdAsync(int categoryId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), categoryId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting roles for category Id '{0}'",
                        categoryId);
                }

                return await _categoryRoleRepository.SelectByCategoryIdAsync(categoryId);
           
            });

        }

        public async Task<bool> DeleteByCategoryIdAsync(int categoryId)
        {
            
            var success = await _categoryRoleRepository.DeleteByCategoryIdAsync(categoryId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted all category roles for category '{0}'",
                        categoryId);
                }
                _cacheManager.CancelTokens(this.GetType());

            }

            return success;

        }
    }

}
