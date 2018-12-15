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
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserBadgeById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    userBadge = new UserBadge();
                    userBadge.PopulateModel(reader);
                }

            }

            return userBadge;
        }

        public async Task<IPagedResults<UserBadge>> SelectAsync(params object[] inputParams)
        {
            PagedResults<UserBadge> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserBadgesPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<UserBadge>();
                    while (await reader.ReadAsync())
                    {
                        var Label = new UserBadge();
                        Label.PopulateModel(reader);
                        output.Data.Add(Label);
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
                _logger.LogInformation($"Deleting user badge with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteUserBadgeById", id);
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
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUserBadge",
                    id,
                    badgeName.ToEmptyIfNull(),
                    userId,
                    createdDate.ToDateIfNull(),
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }
            return output;

        }

        #endregion

    }

}
