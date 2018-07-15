using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Categories.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Repositories;

namespace Plato.Categories.Repositories
{

    public class CategoryRoleRepository : ICategoryRoleRepository<CategoryRole>
    {

        #region "Constructor"

        private readonly ICategoryDataRepository<CategoryData> _categoryDataRepository;

        private readonly IDbContext _dbContext;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRoleRepository(
            IDbContext dbContext,
            ILogger<CategoryRepository> logger,
            ICategoryDataRepository<CategoryData> categoryDataRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _categoryDataRepository = categoryDataRepository;
        }

        #endregion

        #region "Implementation"

        public async Task<CategoryRole> InsertUpdateAsync(CategoryRole model)
        {

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var id = await InsertUpdateInternal(
                model.Id,
                model.CategoryId,
                model.RoleId,
                model.CreatedUserId,
                model.CreatedDate,
                model.ModifiedUserId,
                model.ModifiedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<CategoryRole> SelectByIdAsync(int id)
        {

            CategoryRole categoryRole = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectCategoryRoleById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    categoryRole = new CategoryRole();
                    categoryRole.PopulateModel(reader);
                }

            }

            return categoryRole;

        }

        public async Task<IPagedResults<CategoryRole>> SelectAsync(params object[] inputParams)
        {
            PagedResults<CategoryRole> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectCategoryRolesPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<CategoryRole>();
                    while (await reader.ReadAsync())
                    {
                        var entity = new CategoryRole();
                        entity.PopulateModel(reader);
                        output.Data.Add(entity);
                    }

                    if (await reader.NextResultAsync())
                    {
                        await reader.ReadAsync();
                        output.PopulateTotal(reader);
                    }

                }
            }

            return output;
        }

        public async Task<bool> DeleteAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting category with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteCategoryById", id);
            }

            return success > 0 ? true : false;

        }

        public async Task<IEnumerable<CategoryRole>> SelectByCategoryIdAsync(int categoryId)
        {

            List<CategoryRole> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectCategoryRolesByCategoryId",
                    categoryId
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new List<CategoryRole>();
                    while (await reader.ReadAsync())
                    {
                        var category = new CategoryRole();
                        category.PopulateModel(reader);
                        output.Add(category);
                    }

                }
            }

            return output;
        }

        public async Task<bool> DeleteByCategoryIdAsync(int categoryId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting category roles for category Id: {categoryId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteCategoryRolesByCategoryId", categoryId);
            }

            return success > 0 ? true : false;

        }

        public async Task<bool> DeleteByRoleIdAndCategoryIdAsync(int roleId, int categoryId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting category roles for category Id: {categoryId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteCategoryRolesByRoleIdAndCategoryId",
                    roleId,
                    categoryId);
            }

            return success > 0 ? true : false;

        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int categoryId,
            int roleId,
            int createdUserId,
            DateTime? createdDate,
            int modifiedUserId,
            DateTime? modifiedDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateCategoryRole",
                    id,
                    categoryId,
                    roleId,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    modifiedUserId,
                    modifiedDate.ToDateIfNull());
            }

            return output;

        }

        #endregion

    }

}
