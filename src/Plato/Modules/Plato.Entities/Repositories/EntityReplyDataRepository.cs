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
    
    public class EntityReplyDataRepository : IEntityReplyDataRepository<IEntityReplyData>
    {

        #region Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityDataRepository> _logger;

        #endregion

        #region "Constructor"

        public EntityReplyDataRepository(
            IDbContext dbContext,
            ILogger<EntityDataRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public async Task<IEntityReplyData> SelectByIdAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Selecting entity reply data with id: {id}");
            }
                
            EntityReplyData data = null;
            using (var context = _dbContext)
            {
                data = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityReplyDatumById",
                    async reader =>
                    {
                        if (reader != null)
                        {
                            if (reader.HasRows)
                            {
                                data = new EntityReplyData();
                                await reader.ReadAsync();
                                data.PopulateModel(reader);
                            }
                        }

                        return data;
                    }, new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });


            }

            return data;

        }

        public async Task<IEnumerable<IEntityReplyData>> SelectByReplyIdAsync(int entityId)
        {
            
            IList<EntityReplyData> data = null;
            using (var context = _dbContext)
            {
                data = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityReplyDatumByReplyId",
                    async reader =>
                    {
                        if (reader != null)
                        {
                            if (reader.HasRows)
                            {
                                data = new List<EntityReplyData>();
                                while (await reader.ReadAsync())
                                {
                                    var entityData = new EntityReplyData();
                                    entityData.PopulateModel(reader);
                                    data.Add(entityData);
                                }
                            }
                        }

                        return data;

                    }, new IDbDataParameter[]
                    {
                        new DbParam("ReplyId", DbType.Int32, entityId)
                    });

            }
            return data;

        }

        public async Task<IEntityReplyData> InsertUpdateAsync(IEntityReplyData data)
        {
            var id = await InsertUpdateInternal(
                data.Id,
                data.ReplyId,
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
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityReplyDatumById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }
    
        public async Task<IPagedResults<IEntityReplyData>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<IEntityReplyData> results = null;
            using (var context = _dbContext)
            {
                results = await context.ExecuteReaderAsync<PagedResults<IEntityReplyData>>(
                    CommandType.StoredProcedure,
                    "SelectEntityReplyDatumPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            var output = new PagedResults<IEntityReplyData>();
                            while (await reader.ReadAsync())
                            {
                                var data = new EntityReplyData();
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
            int replyId,
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
                    ? $"Inserting entity reply data with key: {key}"
                    : $"Updating entity reply data with id: {id}");
            }
            
            var output = 0;
            using (var context = _dbContext)
            {
                if (context == null)
                    return 0;
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityReplyDatum",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("ReplyId", DbType.Int32, replyId),
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
