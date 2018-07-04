using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;
using Plato.Entities.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Repositories;
using Plato.Internal.Repositories.Abstract;

namespace Plato.Entities.Repositories
{
    

    public class EntityDataRepository : IEntityDataRepository<EntityData>
    {

        #region Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<DictionaryRepository> _logger;

        #endregion

        #region "Constructor"

        public EntityDataRepository(
            IDbContext dbContext,
            ILogger<DictionaryRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public async Task<EntityData> SelectByIdAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Selecting entity data with id: {id}");

            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                    "SelectEntityDatumById", id);
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        var data = new EntityData();
                        await reader.ReadAsync();
                        data.PopulateModel(reader);
                        return data;
                    }
                }

            }

            return null;

        }

        public async Task<IEnumerable<EntityData>> SelectDataByEntityId(int entityId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Selecting all entity data for id {entityId}");

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

        public async Task<EntityData> InsertUpdateAsync(EntityData data)
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

    

        public Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Deleting user data with id: {id}");
            
            throw new NotImplementedException();
        }
        
        public Task<IPagedResults<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<EntityData>> SelectAsync(params object[] inputParams)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int entityId,
            string key,
            string value,
            DateTime? createdDate,
            int createdUserId,
            DateTime? modifiedDate,
            int modifiedUserId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(id == 0
                    ? $"Inserting user data with key: {key}"
                    : $"Updating user data with id: {id}");
            }

            using (var context = _dbContext)
            {
                if (context == null)
                    return 0;
                return await context.ExecuteScalarAsync<int>(
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

        }

        #endregion

    }
}
