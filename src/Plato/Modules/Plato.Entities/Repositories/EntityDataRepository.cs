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
                data = await context.ExecuteReaderAsync2<EntityData>(
                    CommandType.StoredProcedure,
                    "SelectEntityDatumById",
                    async reader =>
                    {
                        if (reader != null)
                        {
                            if (reader.HasRows)
                            {
                                data = new EntityData();
                                await reader.ReadAsync();
                                data.PopulateModel(reader);
                            }
                        }

                        return data;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });


            }

            return data;

        }

        public async Task<IEnumerable<IEntityData>> SelectByEntityIdAsync(int entityId)
        {
            
            IList<EntityData> data = null;
            using (var context = _dbContext)
            {
                data = await context.ExecuteReaderAsync2<IList<EntityData>>(
                    CommandType.StoredProcedure,
                    "SelectEntityDatumByEntityId",
                    async reader =>
                    {
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

                        return data;

                    }, new[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId)
                    });

            }
            return data;

        }

        public async Task<IEntityData> InsertUpdateAsync(IEntityData data)
        {
            var id = await InsertUpdateInternal(
                data.Id,
                data.EntityId,
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
                    "DeleteEntityDatumById", new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }
    
        public async Task<IPagedResults<IEntityData>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<IEntityData> results = null;
            using (var context = _dbContext)
            {
                results = await context.ExecuteReaderAsync2<PagedResults<IEntityData>>(
                    CommandType.StoredProcedure,
                    "SelectEntityDatumPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            var output = new PagedResults<IEntityData>();
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

                            return output;
                        }

                        return null;

                    },
                    dbParams);

            }

            return results;
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
                output = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityDatum",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("Key", DbType.String, 255, key),
                        new DbParam("Value", DbType.String, value),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("UniqueId",DbType.Int32, ParameterDirection.Output),
                    });
            }

            return output;

        }

        #endregion

    }

}
