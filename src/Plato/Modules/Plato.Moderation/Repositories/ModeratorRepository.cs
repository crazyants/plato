using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Repositories;
using Plato.Moderation.Models;

namespace Plato.Moderation.Repositories
{
    
    public interface IModeratorRepository<T> : IRepository<T> where T : class
    {

    }

    public class ModeratorRepository : IModeratorRepository<Moderator>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<ModeratorRepository> _logger;

        public ModeratorRepository(
            IDbContext dbContext,
            ILogger<ModeratorRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<Moderator> InsertUpdateAsync(Moderator model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var claims = "";
            if (model.Claims != null)
            {
                claims = model.Claims.Serialize();
            }

            var id = await InsertUpdateInternal(
                model.Id,
                model.UserId,
                model.CategoryId,
                claims,
                model.CreatedUserId,
                model.CreatedDate,
                model.ModifiedUserId,
                model.ModifiedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<Moderator> SelectByIdAsync(int id)
        {
            Moderator moderator = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectModeratorById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    moderator = new Moderator();
                    moderator.PopulateModel(reader);
                }

            }

            return moderator;

        }

        public async Task<IPagedResults<Moderator>> SelectAsync(params object[] inputParams)
        {
            PagedResults<Moderator> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectModeratorsPaged",
                    inputParams);
                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<Moderator>();
                    while (await reader.ReadAsync())
                    {
                        var moderator = new Moderator();
                        moderator.PopulateModel(reader);
                        output.Data.Add(moderator);
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
                _logger.LogInformation($"Deleting moderator with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteModeratorById", id);
            }

            return success > 0 ? true : false;
        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int userId,
            int categoryId,
            string claims,
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
                    "InsertUpdateModerator",
                    id,
                    userId,
                    categoryId,
                    claims.ToEmptyIfNull(),
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
