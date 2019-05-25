using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Reactions.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Reactions.Repositories
{
    
    public class EntityReactionRepository : IEntityReactionsRepository<EntityReaction>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityReactionRepository> _logger;

        public EntityReactionRepository(
            IDbContext dbContext,
            ILogger<EntityReactionRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<EntityReaction> InsertUpdateAsync(EntityReaction reaction)
        {
            if (reaction == null)
            {
                throw new ArgumentNullException(nameof(reaction));
            }

            var id = await InsertUpdateInternal(
                reaction.Id,
                reaction.ReactionName,
                reaction.Sentiment,
                reaction.Points,
                reaction.FeatureId,
                reaction.EntityId,
                reaction.EntityReplyId,
                reaction.IpV4Address,
                reaction.IpV6Address,
                reaction.UserAgent,
                reaction.CreatedUserId,
                reaction.CreatedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<EntityReaction> SelectByIdAsync(int id)
        {
            EntityReaction entityReaction = null;
            using (var context = _dbContext)
            {
                entityReaction = await context.ExecuteReaderAsync<EntityReaction>(
                    CommandType.StoredProcedure,
                    "SelectEntityReactionById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            entityReaction = new EntityReaction();
                            entityReaction.PopulateModel(reader);
                        }

                        return entityReaction;
                    },
                    id);

            }

            return entityReaction;

        }

        public async Task<IPagedResults<EntityReaction>> SelectAsync(DbParam[] dbParams)
        {
            IPagedResults<EntityReaction> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync2<IPagedResults<EntityReaction>>(
                    CommandType.StoredProcedure,
                    "SelectEntityReactionsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<EntityReaction>();
                            while (await reader.ReadAsync())
                            {
                                var reaction = new EntityReaction();
                                reaction.PopulateModel(reader);
                                output.Data.Add(reaction);
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
                _logger.LogInformation($"Deleting email with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityReactionById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }


        public async Task<IEnumerable<EntityReaction>> SelectEntityReactionsByEntityId(int entityId)
        {

            IList<EntityReaction> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<EntityReaction>>(
                    CommandType.StoredProcedure,
                    "SelectEntityReactionsByEntityId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<EntityReaction>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityReaction();
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

        public async Task<IEnumerable<EntityReaction>> SelectEntityReactionsByUserIdAndEntityId(int userId, int entityId)
        {

            IList<EntityReaction> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<EntityReaction>>(
                    CommandType.StoredProcedure,
                    "SelectEntityReactionsByUserIdAndEntityId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<EntityReaction>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityReaction();
                                entity.PopulateModel(reader);
                                output.Add(entity);
                            }
                        }

                        return output;
                    },
                    userId,
                    entityId);
            
            }

            return output;

        }
        
        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            string reactionName,
            Sentiment sentiment,
            int points,
            int featureId,
            int entityId,
            int entityReplyId,
            string ipV4Address,
            string ipV6Address,
            string userAgent,
            int createdUserId,
            DateTimeOffset? createdDate)
        {

            var emailId = 0;
            using (var context = _dbContext)
            {
                emailId = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityReaction",
                    new []
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("ReactionName", DbType.String, 255, reactionName),
                        new DbParam("Sentiment", DbType.Int16, sentiment),
                        new DbParam("Points", DbType.Int32, points),
                        new DbParam("FeatureId", DbType.Int32, featureId),
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("EntityReplyId", DbType.Int32, entityReplyId),
                        new DbParam("IpV4Address", DbType.String, 20, ipV4Address),
                        new DbParam("IpV6Address", DbType.String, 50, ipV6Address),
                        new DbParam("UserAgent", DbType.String, 255, userAgent),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }

            return emailId;

        }

        #endregion

    }

}
