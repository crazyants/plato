using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Reactions.Models;

namespace Plato.Reactions.Repositories
{
    
    public class EntityEntityReactionRepository : IEntityReactionRepository<EntityReaction>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityEntityReactionRepository> _logger;

        public EntityEntityReactionRepository(
            IDbContext dbContext,
            ILogger<EntityEntityReactionRepository> logger)
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
                    "SelectReactionById", id);
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
                    "SelectReactionsPaged",
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
                    "DeleteReactionById", id);
            }

            return success > 0 ? true : false;
        }
        
        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            string reactionName,
            Sentiment sentiment,
            int points,
            int entityId,
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
                    createdUserId,
                    createdDate.ToDateIfNull()
                );
            }

            return emailId;

        }

        #endregion
        
    }
}
