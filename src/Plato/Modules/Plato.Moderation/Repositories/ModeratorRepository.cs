using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Moderation.Models;

namespace Plato.Moderation.Repositories
{
    
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
                moderator = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectModeratorById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            moderator = new Moderator();
                            moderator.PopulateModel(reader);
                        }

                        return moderator;
                    },
                    id);

            }

            return moderator;

        }

        public async Task<IPagedResults<Moderator>> SelectAsync(params object[] inputParams)
        {
            IPagedResults<Moderator> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<Moderator>>(
                    CommandType.StoredProcedure,
                    "SelectModeratorsPaged",
                    async reader =>
                    {

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
                _logger.LogInformation($"Deleting moderator with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteModeratorById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
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
                emailId = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateModerator",
                    new []
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("UserId", DbType.Int32, userId),
                        new DbParam("CategoryId", DbType.Int32, categoryId),
                        new DbParam("Claims", DbType.String, claims.ToEmptyIfNull()),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate.ToDateIfNull()),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }

            return emailId;

        }


        #endregion

    }
}
