using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Reactions.Models;

namespace Plato.Reactions.Repositories
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

        public async Task<EntityReaction> InsertUpdateAsync(EntityReaction entityReaction)
        {
            if (entityReaction == null)
            {
                throw new ArgumentNullException(nameof(entityReaction));
            }

            var id = await InsertUpdateInternal(
                entityReaction.Id,
                entityReaction.ReactionName,
                entityReaction.Sentiment,
                entityReaction.Points,
                entityReaction.EntityId,
                entityReaction.EntityReplyId,
                entityReaction.CreatedUserId,
                entityReaction.CreatedDate);

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
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityReactionById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    entityReaction = new EntityReaction();
                    entityReaction.PopulateModel(reader);
                }

            }

            return entityReaction;
        }

        public async Task<IPagedResults<EntityReaction>> SelectAsync(params object[] inputParams)
        {
            PagedResults<EntityReaction> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityReactionsPaged",
                    inputParams);
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

            List<EntityReaction> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityReactionsByEntityId",
                    entityId);
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
            }

            return output;

        }

        public async Task<IEnumerable<EntityReaction>> SelectEntityReactionsByUserIdAndEntityId(int userId, int entityId)
        {

            List<EntityReaction> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityReactionsByUserIdAndEntityId",
                    userId,
                    entityId);
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
            int entityId,
            int entityReplyId,
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
                    entityId,
                    entityReplyId,
                    createdUserId,
                    createdDate.ToDateIfNull()
                );
            }

            return emailId;

        }

        #endregion

    }

}
