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

    public class CategoryDataRepository : ICategoryDataRepository<CategoryData>
    {

        #region Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<CategoryDataRepository> _logger;

        #endregion

        #region "Constructor"

        public CategoryDataRepository(
            IDbContext dbContext,
            ILogger<CategoryDataRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public async Task<CategoryData> SelectByIdAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Selecting entity data with id: {id}");
            }

            CategoryData data = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                    "SelectCategoryDatumById", id);
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        data = new CategoryData();
                        await reader.ReadAsync();
                        data.PopulateModel(reader);
                    }
                }

            }

            return data;

        }

        public async Task<IEnumerable<CategoryData>> SelectByCategoryIdAsync(int categoryId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Selecting all category data for id {categoryId}");
            }

            List<CategoryData> data = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectCategoryDatumByCategoryId",
                    categoryId);
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        data = new List<CategoryData>();
                        while (await reader.ReadAsync())
                        {
                            var entityData = new CategoryData();
                            entityData.PopulateModel(reader);
                            data.Add(entityData);
                        }
                    }
                }
            }
            return data;

        }

        public async Task<CategoryData> InsertUpdateAsync(CategoryData data)
        {
            var id = await InsertUpdateInternal(
                data.Id,
                data.CategoryId,
                data.Key.ToEmptyIfNull().TrimToSize(255),
                data.Value.ToEmptyIfNull(),
                data.CreatedDate.ToDateIfNull(),
                data.CreatedUserId,
                data.ModifiedDate.ToDateIfNull(),
                data.ModifiedUserId);
            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }
                
            return null;
        }

        public async Task<bool> DeleteAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting entity data id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteCategoryDatumById", id);
            }

            return success > 0 ? true : false;

        }

        public async Task<IPagedResults<CategoryData>> SelectAsync(params object[] inputParams)
        {
            PagedResults<CategoryData> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectCategoryDatumPaged",
                    inputParams);
                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<CategoryData>();
                    while (await reader.ReadAsync())
                    {
                        var data = new CategoryData();
                        data.PopulateModel(reader);
                        output.Data.Add(data);
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

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int categoryId,
            string key,
            string value,
            DateTimeOffset? createdDate,
            int createdUserId,
            DateTimeOffset? modifiedDate,
            int modifiedUserId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(id == 0
                    ? $"Inserting category data with key: {key}"
                    : $"Updating category data with id: {id}");
            }

            var output = 0;
            using (var context = _dbContext)
            {

                if (context == null)
                {
                    return output;
                }
                    
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateCategoryDatum",
                    id,
                    categoryId,
                    key.ToEmptyIfNull().TrimToSize(255),
                    value.ToEmptyIfNull(),
                    createdDate.ToDateIfNull(),
                    createdUserId,
                    modifiedDate.ToDateIfNull(),
                    modifiedUserId,
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }

            return output;

        }

        #endregion

    }



}
