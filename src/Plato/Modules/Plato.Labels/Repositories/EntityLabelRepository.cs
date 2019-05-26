using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Labels.Models;

namespace Plato.Labels.Repositories
{

    public class EntityLabelRepository : IEntityLabelRepository<EntityLabel>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityLabelRepository> _logger;

        public EntityLabelRepository(
            IDbContext dbContext,
            ILogger<EntityLabelRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<EntityLabel> InsertUpdateAsync(EntityLabel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var id = await InsertUpdateInternal(
                model.Id,
                model.EntityId,
                model.LabelId,
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

        public async Task<EntityLabel> SelectByIdAsync(int id)
        {
            EntityLabel output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityLabelById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            output = new EntityLabel();
                            output.PopulateModel(reader);
                        }

                        return output;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });


            }

            return output;

        }

        public async Task<IPagedResults<EntityLabel>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<EntityLabel> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<EntityLabel>>(
                    CommandType.StoredProcedure,
                    "SelectEntityLabelsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<EntityLabel>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityLabel();
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
                _logger.LogInformation($"Deleting entity Label relationship with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityLabelById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<IEnumerable<EntityLabel>> SelectByEntityIdAsync(int entityId)
        {
            IList<EntityLabel> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<EntityLabel>>(
                    CommandType.StoredProcedure,
                    "SelectEntityLabelsByEntityId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<EntityLabel>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityLabel();
                                entity.PopulateModel(reader);
                                output.Add(entity);
                            }

                        }

                        return output;
                    }, new[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId)
                    });

            }

            return output;
        }

        public async Task<bool> DeleteByEntityIdAsync(int entityId)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting all entity label relationships for entity id: {entityId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityLabelsByEntityId",
                    new[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId)
                    });
            }

            return success > 0 ? true : false;
        }

        public async Task<bool> DeleteByEntityIdAndLabelIdAsync(int entityId, int labelId)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(
                    $"Deleting entity Label relationship with entityId '{entityId}' and labelId '{labelId}'");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityLabelByEntityIdAndLabelId",
                    new[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("LabelId", DbType.Int32, labelId)
                    });
            }

            return success > 0 ? true : false;
        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int entityId,
            int labelId,
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
                    "InsertUpdateEntityLabel",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("LabelId", DbType.Int32, labelId),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output)
                    });
            }

            return output;

        }

        #endregion

    }

}
