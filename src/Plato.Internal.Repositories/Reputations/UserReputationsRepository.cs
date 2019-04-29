using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Reputations;

namespace Plato.Internal.Repositories.Reputations
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
                model.FeatureId,
                model.Name,
                model.Description,
                model.Points,
                model.CreatedUserId,
                model.CreatedDate
             );

            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<UserReputation> SelectByIdAsync(int id)
        {
            UserReputation userReputation = null;
            using (var context = _dbContext)
            {
                userReputation = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserReputationById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            userReputation = new UserReputation();
                            await reader.ReadAsync();
                            userReputation.PopulateModel(reader);
                        }

                        return userReputation;
                    },
                    id);
             

            }

            return userReputation;
        }

        public async Task<IPagedResults<UserReputation>> SelectAsync(params object[] inputParams)
        {
            IPagedResults<UserReputation> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<UserReputation>>(
                    CommandType.StoredProcedure,
                    "SelectUserReputationsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<UserReputation>();
                            while (await reader.ReadAsync())
                            {
                                var userReputation = new UserReputation();
                                userReputation.PopulateModel(reader);
                                output.Data.Add(userReputation);
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
                    inputParams);

              
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
            int featureId,
            string name,
            string description,
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
                    featureId,
                    name.ToEmptyIfNull().TrimToSize(255),
                    description.ToEmptyIfNull().TrimToSize(255),
                    points,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }
            return output;

        }

        #endregion

    }

}
