using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Reactions.Models;

namespace Plato.Reactions.Repositories
{
    
    public class ReactionRepository : IReactionRepository<Reaction>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<ReactionRepository> _logger;

        public ReactionRepository(
            IDbContext dbContext,
            ILogger<ReactionRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<Reaction> InsertUpdateAsync(Reaction reaction)
        {
            if (reaction == null)
            {
                throw new ArgumentNullException(nameof(reaction));
            }

            var id = await InsertUpdateInternal(
                reaction.Id,
                reaction.FeatureId,
                reaction.Name,
                reaction.Description,
                reaction.Emoji,
                reaction.IsPositive,
                reaction.IsNeutral,
                reaction.IsNegative,
                reaction.IsDisabled,
                reaction.CreatedUserId,
                reaction.CreatedDate,
                reaction.ModifiedUserId,
                reaction.ModifiedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<Reaction> SelectByIdAsync(int id)
        {
            Reaction reaction = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectReactionById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    reaction = new Reaction();
                    reaction.PopulateModel(reader);
                }

            }

            return reaction;
        }

        public async Task<IPagedResults<Reaction>> SelectAsync(params object[] inputParams)
        {
            PagedResults<Reaction> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectReactionsPaged",
                    inputParams);
                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<Reaction>();
                    while (await reader.ReadAsync())
                    {
                        var reaction = new Reaction();
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
