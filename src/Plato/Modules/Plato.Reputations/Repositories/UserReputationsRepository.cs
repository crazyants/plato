using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Reputations.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Reputations.Repositories
{
    public class UserReputationsRepository : IUserReputationsRepository<UserReputation>
    {
        
        private readonly IDbContext _dbContext;
        private readonly ILogger<UserReputationsRepository> _logger;
        
        public UserReputationsRepository(
            IDbContext dbContext, 
            ILogger<UserReputationsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<UserReputation> InsertUpdateAsync(UserReputation model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var id = await InsertUpdateInternal(
                model.Id,
                model.Name,
                model.Points,
                model.CreatedUserId,
                model.CreatedDate
             );

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<UserReputation> SelectByIdAsync(int id)
        {
            UserReputation userBadge = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserReputationById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    userBadge = new UserReputation();
                    userBadge.PopulateModel(reader);
                }

            }

            return userBadge;
        }

        public async Task<IPagedResults<UserReputation>> SelectAsync(params object[] inputParams)
        {
            PagedResults<UserReputation> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserReputationsPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<UserReputation>();
                    while (await reader.ReadAsync())
                    {
                        var Label = new UserReputation();
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
                _logger.LogInformation($"Deleting user reputation with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteUserReputationById", id);
            }

            return success > 0 ? true : false;
        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            string name,
            int points,
            int createdUserId,
            DateTimeOffset? createdDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUserReputation",
                    id,
                    name.ToEmptyIfNull(),
                    points,
                    createdUserId,
                    createdDate.ToDateIfNull());
            }
            return output;

        }

        #endregion

    }

}
