using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Categories.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Categories.Repositories
{

    public class CategoryRoleRepository : ICategoryRoleRepository<CategoryRole>
    {

        #region "Constructor"

        private readonly ICategoryDataRepository<CategoryData> _categoryDataRepository;

        private readonly IDbContext _dbContext;
        private readonly ILogger<CategoryRoleRepository> _logger;

        public CategoryRoleRepository(
            IDbContext dbContext,
            ILogger<CategoryRoleRepository> logger,
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
                categoryRole = await context.ExecuteReaderAsync<CategoryRole>(
                    CommandType.StoredProcedure,
                    "SelectCategoryRoleById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            categoryRole = new CategoryRole();
                            await reader.ReadAsync();
                            categoryRole.PopulateModel(reader);
                        }

                        return categoryRole;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

            }

            return categoryRole;

        }

        public async Task<IPagedResults<CategoryRole>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<CategoryRole> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<CategoryRole>>(
                    CommandType.StoredProcedure,
                    "SelectCategoryRolesPaged",
                    async reader =>
                    {
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
                                if (reader.HasRows)
                                {
                                    await reader.ReadAsync();
                                    output.PopulateTotal(reader);
                                }
                              
                            }

                        }

                        return output;
                    },
                    dbParams
                );

           
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
                    "DeleteCategoryById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<IEnumerable<CategoryRole>> SelectByCategoryIdAsync(int categoryId)
        {

            IList<CategoryRole> output = null;
            using (var context = _dbContext)
            {

                output = await context.ExecuteReaderAsync<IList<CategoryRole>>(
                    CommandType.StoredProcedure,
                    "SelectCategoryRolesByCategoryId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<CategoryRole>();
                            while (await reader.ReadAsync())
                            {
                                var categoryRole = new CategoryRole();
                                categoryRole.PopulateModel(reader);
                                output.Add(categoryRole);
                            }

                        }

                        return output;
                    }, new[]
                    {
                        new DbParam("CategoryId", DbType.Int32, categoryId)
                    });

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
                    "DeleteCategoryRolesByCategoryId",
                    new[]
                    {
                        new DbParam("CategoryId", DbType.Int32, categoryId)
                    });
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
                    new[]
                    {
                        new DbParam("RoleId", DbType.Int32, roleId),
                        new DbParam("CategoryId", DbType.Int32, categoryId),
                    });
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
            DateTimeOffset? createdDate,
            int modifiedUserId,
            DateTimeOffset? modifiedDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateCategoryRole",
                    new []
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("CategoryId", DbType.Int32, categoryId),
                        new DbParam("RoleId", DbType.Int32, roleId),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate.ToDateIfNull()),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }

            return output;

        }

        #endregion

    }

}
