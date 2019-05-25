using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Internal.Repositories.Badges
{
    public class UserBadgeRepository : IUserBadgeRepository<UserBadge>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<UserBadgeRepository> _logger;

        public UserBadgeRepository(
            IDbContext dbContext,
            ILogger<UserBadgeRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<UserBadge> InsertUpdateAsync(UserBadge model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var id = await InsertUpdateInternal(
                model.Id,
                model.BadgeName,
                model.UserId,
                model.CreatedDate
            );

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<UserBadge> SelectByIdAsync(int id)
        {
            UserBadge userBadge = null;
            using (var context = _dbContext)
            {
                userBadge = await context.ExecuteReaderAsync2<UserBadge>(
                    CommandType.StoredProcedure,
                    "SelectUserBadgeById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            userBadge = new UserBadge();
                            await reader.ReadAsync();
                            userBadge.PopulateModel(reader);
                        }

                        return userBadge;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });


            }

            return userBadge;
        }

        public async Task<IPagedResults<UserBadge>> SelectAsync(DbParam[] dbParams)
        {
            PagedResults<UserBadge> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync2(
                    CommandType.StoredProcedure,
                    "SelectUserBadgesPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<UserBadge>();
                            while (await reader.ReadAsync())
                            {
                                var userBadge = new UserBadge();
                                userBadge.PopulateModel(reader);
                                output.Data.Add(userBadge);
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
                _logger.LogInformation($"Deleting user badge with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteUserBadgeById",
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
            string badgeName,
            int userId,
            DateTimeOffset? createdDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUserBadge",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("BadgeName", DbType.String, 255, badgeName),
                        new DbParam("UserId", DbType.Int32, userId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output)
                    });
            }

            return output;

        }

        #endregion

    }

}
