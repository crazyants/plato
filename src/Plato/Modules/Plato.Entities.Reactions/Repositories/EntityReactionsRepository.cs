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

        public async Task<IPagedResults<EntityReaction>> SelectAsync(params object[] inputParams)
        {
            IPagedResults<EntityReaction> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<EntityReaction>>(
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
                    inputParams);
            
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
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityReactionById", id);
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
                emailId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityReaction",
                    id,
                    reactionName.ToEmptyIfNull().TrimToSize(255),
                    (short)sentiment,
                    points,
                    featureId,
                    entityId,
                    entityReplyId,
                    ipV4Address.TrimToSize(20).ToEmptyIfNull(),
                    ipV6Address.TrimToSize(50).ToEmptyIfNull(),
                    userAgent.TrimToSize(255).ToEmptyIfNull(),
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output)
                );
            }

            return emailId;

        }

        #endregion

    }

}
