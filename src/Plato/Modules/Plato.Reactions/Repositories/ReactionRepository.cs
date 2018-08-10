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
                reaction.IsDisabled);

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

        public Task<IPagedResults<Reaction>> SelectAsync(params object[] inputParams)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
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
            bool isDeisabled)
        {

            var emailId = 0;
            using (var context = _dbContext)
            {
                emailId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateReaction",
                    id,
                    name.ToEmptyIfNull().TrimToSize(255),
                    description.ToEmptyIfNull().TrimToSize(255),
                    emoji.ToEmptyIfNull().TrimToSize(255),
                    isPositive,
                    isNeutral,
                    isNegative,
                    isDeisabled);
            }

            return emailId;

        }

        #endregion


    }
}
