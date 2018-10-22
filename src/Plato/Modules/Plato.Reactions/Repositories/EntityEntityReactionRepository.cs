using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Reactions.Models;

namespace Plato.Reactions.Repositories
{
    
    public class EntityEntityReactionRepository : IEntityReactionRepository<EntityReacttion>
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

        public async Task<EntityReacttion> InsertUpdateAsync(EntityReacttion entityReacttion)
        {
            if (entityReacttion == null)
            {
                throw new ArgumentNullException(nameof(entityReacttion));
            }

            var id = await InsertUpdateInternal(
                entityReacttion.Id,
                entityReacttion.FeatureId,
                entityReacttion.Name,
                entityReacttion.Description,
                entityReacttion.Emoji,
                entityReacttion.IsPositive,
                entityReacttion.IsNeutral,
                entityReacttion.IsNegative,
                entityReacttion.IsDisabled,
                entityReacttion.CreatedUserId,
                entityReacttion.CreatedDate,
                entityReacttion.ModifiedUserId,
                entityReacttion.ModifiedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<EntityReacttion> SelectByIdAsync(int id)
        {
            EntityReacttion entityReacttion = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectReactionById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    entityReacttion = new EntityReacttion();
                    entityReacttion.PopulateModel(reader);
                }

            }

            return entityReacttion;
        }

        public async Task<IPagedResults<EntityReacttion>> SelectAsync(params object[] inputParams)
        {
            PagedResults<EntityReacttion> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectReactionsPaged",
                    inputParams);
                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<EntityReacttion>();
                    while (await reader.ReadAsync())
                    {
                        var reaction = new EntityReacttion();
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
            int featureId,
            string name,
            string description,
            string emoji,
            bool isPositive,
            bool isNeutral,
            bool isNegative,
            bool isDeisabled,
            int createdUserId,
            DateTimeOffset? createdDate,
            int modifiedUserId,
            DateTimeOffset? modifiedDate)
        {

            var emailId = 0;
            using (var context = _dbContext)
            {
                emailId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateReaction",
                    id,
                    featureId,
                    name.ToEmptyIfNull().TrimToSize(255),
                    description.ToEmptyIfNull().TrimToSize(255),
                    emoji.ToEmptyIfNull().TrimToSize(20),
                    isPositive,
                    isNeutral,
                    isNegative,
                    isDeisabled,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    modifiedUserId,
                    modifiedDate.ToDateIfNull());
            }

            return emailId;

        }

        #endregion
        
    }
}
