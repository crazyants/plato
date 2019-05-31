using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Mentions.Models;

namespace Plato.Mentions.Repositories
{

    public class EntityMentionsRepository : IEntityMentionsRepository<EntityMention>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityMentionsRepository> _logger;

        public EntityMentionsRepository(
            IDbContext dbContext,
            ILogger<EntityMentionsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<EntityMention> InsertUpdateAsync(EntityMention model)
        {
            var id = await InsertUpdateInternal(
                model.Id,
                model.EntityId,
                model.EntityReplyId,
                model.UserId,
                model.CreatedUserId,
                model.CreatedDate);

            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }

            return null;

        }

        public async Task<EntityMention> SelectByIdAsync(int id)
        {

            EntityMention output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<EntityMention>(
                    CommandType.StoredProcedure,
                    "SelectEntityMentionById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            output = new EntityMention();
                            output.PopulateModel(reader);
                        }

                        return output;
                    }, new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

            }

            return output;

        }

        public async Task<IPagedResults<EntityMention>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<EntityMention> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<EntityMention>>(
                    CommandType.StoredProcedure,
                    "SelectEntityMentionsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<EntityMention>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityMention();
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
                    dbParams);
                
            }

            return output;
        }

        public async Task<bool> DeleteAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting entity mention with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityMentionById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<bool> DeleteByEntityIdAsync(int entityId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting all mentions for entity Id {entityId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityMentionsByEntityId",
                    new IDbDataParameter[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<bool> DeleteByEntityReplyIdAsync(int entityReplyId)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting all mentions for entity reply with Id {entityReplyId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityMentionsByEntityReplyId",
                    new IDbDataParameter[]
                    {
                        new DbParam("EntityReplyId", DbType.Int32, entityReplyId)
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
            int userId,
            int createdUserId,
            DateTimeOffset? createdDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityMention",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("EntityReplyId", DbType.Int32, entityReplyId),
                        new DbParam("UserId", DbType.Int32, userId),
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
