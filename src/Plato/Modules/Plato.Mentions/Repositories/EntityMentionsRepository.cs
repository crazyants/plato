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
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityMentionById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    output = new EntityMention();
                    output.PopulateModel(reader);
                }

            }

            return output;

        }

        public Task<IPagedResults<EntityMention>> SelectAsync(params object[] inputParams)
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
            int entityId,
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
                    id,
                    entityId,
                    userId,
                    createdUserId,
                    createdDate.ToDateIfNull());
            }

            return output;

        }
        
        #endregion

    }

}
