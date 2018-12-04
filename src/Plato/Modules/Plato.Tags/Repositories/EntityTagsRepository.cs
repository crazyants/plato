using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Tags.Models;

namespace Plato.Tags.Repositories
{

    public class EntityTagsRepository : IEntityTagsRepository<EntityTag>
    {
        
        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityTagsRepository> _logger;

        public EntityTagsRepository(
            IDbContext dbContext,
            ILogger<EntityTagsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        #region "Implementation"

        public async Task<EntityTag> InsertUpdateAsync(EntityTag model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var id = await InsertUpdateInternal(
                model.Id,
                model.EntityId,
                model.TagId,
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

        public async Task<EntityTag> SelectByIdAsync(int id)
        {
            EntityTag output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityTagById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    output = new EntityTag();
                    output.PopulateModel(reader);
                }

            }

            return output;
        }

        public async Task<IPagedResults<EntityTag>> SelectAsync(params object[] inputParams)
        {
            PagedResults<EntityTag> output = null;
            using (var context = _dbContext)
            {

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityTagsPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<EntityTag>();
                    while (await reader.ReadAsync())
                    {
                        var entity = new EntityTag();
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
                _logger.LogInformation($"Deleting entity tag relationship with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityTagById", id);
            }

            return success > 0 ? true : false;
        }

        public async Task<IEnumerable<EntityTag>> SelectByEntityId(int entityId)
        {
            List<EntityTag> output = null;
            using (var context = _dbContext)
            {

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityTagsByEntityId",
                    entityId);

                if ((reader != null) && (reader.HasRows))
                {
                    output = new List<EntityTag>();
                    while (await reader.ReadAsync())
                    {
                        var entity = new EntityTag();
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
                _logger.LogInformation($"Deleting all entity tag relationships for entity id: {entityId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityTagsByEntityId", entityId);
            }

            return success > 0 ? true : false;
        }

        public async Task<bool> DeleteByEntityIdAndTagId(int entityId, int tagId)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting entity tag relationship with entityId '{entityId}' and tagId '{tagId}'");
            }

            var success = 0;
            using (var context = _dbContext)
            {
              
                   success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityTagByEntityIdAndTagId",
                    entityId,
                    tagId);
            }

            return success > 0 ? true : false;
        }
        
        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int entityId,
            int tagId,
            int createdUserId,
            DateTime? createdDate,
            int modifiedUserId,
            DateTime? modifiedDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityTag",
                    id,
                    entityId,
                    tagId,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    modifiedUserId,
                    modifiedDate.ToDateIfNull(),
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }

            return output;

        }

        #endregion
    }

}
