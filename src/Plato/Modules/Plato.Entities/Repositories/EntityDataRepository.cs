using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;
using Plato.Entities.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Repositories
{
    
    public class EntityDataRepository : IEntityDataRepository<IEntityData>
    {

        #region Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityDataRepository> _logger;

        #endregion

        #region "Constructor"

        public EntityDataRepository(
            IDbContext dbContext,
            ILogger<EntityDataRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public async Task<IEntityData> SelectByIdAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Selecting entity data with id: {id}");
            }
                
            EntityData data = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                    "SelectEntityDatumById", id);
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        data = new EntityData();
                        await reader.ReadAsync();
                        data.PopulateModel(reader);
                    }
                }

            }

            return data;

        }

        public async Task<IEnumerable<IEntityData>> SelectByEntityIdAsync(int entityId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Selecting all entity data for id {entityId}");
            }
                
            List<EntityData> data = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityDatumByEntityId",
                    entityId);
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        data = new List<EntityData>();
                        while (await reader.ReadAsync())
                        {
                            var entityData = new EntityData();
                            entityData.PopulateModel(reader);
                            data.Add(entityData);
                        }
                    }
                }
            }
            return data;

        }

        public async Task<IEntityData> InsertUpdateAsync(IEntityData data)
        {
            var id = await InsertUpdateInternal(
                data.Id,
                data.EntityId,
                data.Key.ToEmptyIfNull().TrimToSize(255),
                data.Value.ToEmptyIfNull(),
                data.CreatedDate.ToDateIfNull(),
                data.CreatedUserId,
                data.ModifiedDate.ToDateIfNull(),
                data.ModifiedUserId);
            if (id > 0)
                return await SelectByIdAsync(id);
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
                    "DeleteEntityDatumById", id);
            }

            return success > 0 ? true : false;

        }
    
        public async Task<IPagedResults<IEntityData>> SelectAsync(params object[] inputParams)
        {
            PagedResults<IEntityData> output = null;
            using (var context = _dbContext)
            {
             
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityDatumPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<IEntityData>();
                    while (await reader.ReadAsync())
                    {
                        var data = new EntityData();
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
            int entityId,
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
                    ? $"Inserting entity data with key: {key}"
                    : $"Updating entity data with id: {id}");
            }
            
            var output = 0;
            using (var context = _dbContext)
            {
                if (context == null)
                    return 0;
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityDatum",
                    id,
                    entityId,
                    key.ToEmptyIfNull().TrimToSize(255),
                    value.ToEmptyIfNull(),
                    createdDate.ToDateIfNull(),
                    createdUserId,
                    modifiedDate.ToDateIfNull(),
                    modifiedUserId);
            }

            return output;

        }

        #endregion

    }

}
