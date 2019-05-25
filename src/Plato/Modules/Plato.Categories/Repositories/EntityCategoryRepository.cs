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

    public class EntityCategoryRepository : IEntityCategoryRepository<EntityCategory>
    {
        
        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityCategoryRepository> _logger;

        public EntityCategoryRepository(
            IDbContext dbContext,
            ILogger<EntityCategoryRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<EntityCategory> InsertUpdateAsync(EntityCategory model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var id = await InsertUpdateInternal(
                model.Id,
                model.EntityId,
                model.CategoryId,
                model.CreatedUserId,
                model.CreatedDate,
                model.ModifiedUserId,
                model.ModifiedDate);

            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }

            return null;

        }

        public async Task<EntityCategory> SelectByIdAsync(int id)
        {
            EntityCategory output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<EntityCategory>(
                    CommandType.StoredProcedure,
                    "SelectEntityCategoryById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            output = new EntityCategory();
                            output.PopulateModel(reader);
                        }

                        return output;
                    },
                    id);
             

            }

            return output;

        }

        public async Task<IPagedResults<EntityCategory>> SelectAsync(params object[] inputParams)
        {
            IPagedResults<EntityCategory> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<EntityCategory>>(
                    CommandType.StoredProcedure,
                    "SelectEntityCategoriesPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<EntityCategory>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityCategory();
                                entity.PopulateModel(reader);
                                output.Data.Add(entity);
                            }

                            if (await reader.NextResultAsync())
                            {
                                await reader.ReadAsync();
                                output.PopulateTotal(reader);
                            }

                        }

                        return output;

                    },
                    inputParams
                );

           
            }

            return output;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting entity category relationship with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityCategoryById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<IEnumerable<EntityCategory>> SelectByEntityIdAsync(int entityId)
        {
            IList<EntityCategory> output = null;
            using (var context = _dbContext)
            {

                output = await context.ExecuteReaderAsync<IList<EntityCategory>>(
                    CommandType.StoredProcedure,
                    "SelectEntityCategoriesByEntityId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<EntityCategory>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityCategory();
                                entity.PopulateModel(reader);
                                output.Add(entity);
                            }

                        }

                        return output;
                    },
                    entityId);

              
            }

            return output;
        }

        public async Task<EntityCategory> SelectByEntityIdAndCategoryIdAsync(int entityId, int categoryId)
        {
            EntityCategory output = null;
            using (var context = _dbContext)
            {

                output = await context.ExecuteReaderAsync<EntityCategory>(
                    CommandType.StoredProcedure,
                    "SelectEntityCategoryByEntityIdAndCategoryId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            output = new EntityCategory();
                            output.PopulateModel(reader);
                        }

                        return output;
                    },
                    entityId,
                    categoryId);
                
            }

            return output;
        }

        public async Task<bool> DeleteByEntityIdAsync(int entityId)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting all entity category relationships for entity id: {entityId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityCategoriesByEntityId",
                    new []
                    {
                        new DbParam("EntityId", DbType.Int32, entityId)
                    });
            }

            return success > 0 ? true : false;
        }

        public async Task<bool> DeleteByEntityIdAndCategoryIdAsync(int entityId, int categoryId)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting entity category relationship with entityId '{entityId}' and categoryId '{categoryId}'");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityCategoryByEntityIdAndCategoryId",
                    new[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("CategoryId", DbType.Int32, categoryId),
                    });
            }

            return success > 0 ? true : false;
        }

        #endregion

        #region "Private Methods"
        
        async Task<int> InsertUpdateInternal(
            int id,
            int entityId,
            int categoryId,
            int createdUserId,
            DateTimeOffset? createdDate,
            int modifiedUserId,
            DateTimeOffset? modifiedDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityCategory",
                    new []
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("CategoryId", DbType.Int32, categoryId),
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
