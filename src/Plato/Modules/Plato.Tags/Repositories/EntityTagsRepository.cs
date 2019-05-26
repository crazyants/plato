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
                model.EntityReplyId,
                model.TagId,
                model.CreatedUserId,
                model.CreatedDate);

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
                output = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityTagById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            output = new EntityTag();
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

        public async Task<IPagedResults<EntityTag>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<EntityTag> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<EntityTag>>(
                    CommandType.StoredProcedure,
                    "SelectEntityTagsPaged",
                    async reader =>
                    {
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

                        return output;

                    },
                    dbParams);

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
                    "DeleteEntityTagById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }

        public async Task<IEnumerable<EntityTag>> SelectByEntityId(int entityId)
        {
            IList<EntityTag> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<EntityTag>>(
                    CommandType.StoredProcedure,
                    "SelectEntityTagsByEntityId",
                    async reader =>
                    {
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

                        return output;

                    }, new[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId)
                    });

            }

            return output;
        }

        public async Task<IEnumerable<EntityTag>> SelectByEntityReplyId(int entityReplyId)
        {
            IList<EntityTag> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<EntityTag>>(
                    CommandType.StoredProcedure,
                    "SelectEntityTagsByEntityReplyId",
                    async reader =>
                    {
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

                        return output;
                    },
                    new[]
                    {
                        new DbParam("EntityReplyId", DbType.Int32, entityReplyId)
                    });

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
                    "DeleteEntityTagsByEntityId",
                    new[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId)
                    });
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
                    new[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("TagId", DbType.Int32, tagId),
                    });
            }

            return success > 0 ? true : false;
        }
        
        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int entityId,
            int entityReplyId,
            int tagId,
            int createdUserId,
            DateTimeOffset? createdDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityTag",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("EntityReplyId", DbType.Int32, entityReplyId),
                        new DbParam("TagId", DbType.Int32, tagId),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }

            return output;

        }

        #endregion
    }

}
