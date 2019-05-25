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

            CategoryData data = null;
            using (var context = _dbContext)
            {
                data = await context.ExecuteReaderAsync<CategoryData>(
                    CommandType.StoredProcedure,
                    "SelectCategoryDatumById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            data = new CategoryData();
                            await reader.ReadAsync();
                            data.PopulateModel(reader);
                        }

                        return data;

                    },
                    id);

            }

            return data;

        }

        public async Task<IEnumerable<CategoryData>> SelectByCategoryIdAsync(int categoryId)
        {
            
            List<CategoryData> data = null;
            using (var context = _dbContext)
            {
                data = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectCategoryDatumByCategoryId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            data = new List<CategoryData>();
                            while (await reader.ReadAsync())
                            {
                                var entityData = new CategoryData();
                                entityData.PopulateModel(reader);
                                data.Add(entityData);
                            }
                        }

                        return data;
                    },
                    categoryId);

            }
            return data;

        }

        public async Task<CategoryData> InsertUpdateAsync(CategoryData data)
        {
            var id = await InsertUpdateInternal(
                data.Id,
                data.CategoryId,
                data.Key,
                data.Value,
                data.CreatedDate,
                data.CreatedUserId,
                data.ModifiedDate,
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
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteCategoryDatumById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<IPagedResults<CategoryData>> SelectAsync(DbParam[] dbParams)
        {
            IPagedResults<CategoryData> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync2<IPagedResults<CategoryData>>(
                    CommandType.StoredProcedure,
                    "SelectCategoryDatumPaged",
                    async reader =>
                    {
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
                                if (reader.HasRows)
                                {
                                    await reader.ReadAsync();
                                    output.PopulateTotal(reader);
                                }
                            }

                        }

                        return output;
                    },
                    dbParams);
              
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
                    
                output = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateCategoryDatum",
                    new []
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("CategoryId", DbType.Int32, categoryId),
                        new DbParam("Key", DbType.String, 255, key.ToEmptyIfNull()),
                        new DbParam("Value", DbType.String, value.ToEmptyIfNull()),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.Int32, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.Int32, modifiedDate.ToDateIfNull()),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }

            return output;

        }

        #endregion

    }



}
