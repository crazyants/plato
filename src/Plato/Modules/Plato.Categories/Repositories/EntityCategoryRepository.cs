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
                // return
                return await SelectByIdAsync(id);
            }

            return null;

        }

        public async Task<EntityCategory> SelectByIdAsync(int id)
        {
            EntityCategory output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityCategoryById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    output = new EntityCategory();
                    output.PopulateModel(reader);
                }

            }

            return output;

        }

        public async Task<IPagedResults<EntityCategory>> SelectAsync(params object[] inputParams)
        {
            PagedResults<EntityCategory> output = null;
            using (var context = _dbContext)
            {

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityCategoriesPaged",
                    inputParams
                );

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
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityCategoryById", id);
            }

            return success > 0 ? true : false;

        }

        public async Task<IEnumerable<EntityCategory>> SelectByEntityId(int entityId)
        {
            List<EntityCategory> output = null;
            using (var context = _dbContext)
            {

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityCategoriesByEntityId",
                    entityId);

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
            }

            return output;
        }

        public async Task<bool> DeleteByEntityId(int entityId)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting all entity category relationships for entity id: {entityId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityCategoriesByEntityId", entityId);
            }

            return success > 0 ? true : false;
        }

        public async Task<bool> DeleteByEntityIdAndCategoryId(int entityId, int categoryId)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting entity category relationship with entityId '{entityId}' and categoryId '{categoryId}'");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityCategoryByEntityIdAndCategoryId",
                    entityId,
                    categoryId);
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
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityCategory",
                    id,
                    entityId,
                    categoryId,
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
