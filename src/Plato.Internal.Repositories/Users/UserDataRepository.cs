using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Repositories.Abstract;

namespace Plato.Internal.Repositories.Users
{
    public class UserDataRepository : IUserDataRepository<UserData>
    {

        #region Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<DictionaryRepository> _logger;

        #endregion

        #region "Constructor"

        public UserDataRepository(
            IDbContext dbContext,
            ILogger<DictionaryRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public async Task<UserData> SelectByIdAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Selecting setting with id: {id}");

            UserData userData = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserDatumById", id);
                if (reader != null && reader.HasRows)
                {
                    userData = new UserData();
                    await reader.ReadAsync();
                    userData.PopulateModel(reader);
                }

            }

            return userData;

        }

        public async Task<UserData> SelectByKeyAndUserIdAsync(string key, int userId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Selecting all user data for userId {userId}");
            }

            UserData data = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserDatumByKeyAndUserId",
                    key.ToEmptyIfNull(),
                    userId);
                if (reader != null && reader.HasRows)
                {
                    await reader.ReadAsync();
                    data = new UserData();
                    data.PopulateModel(reader);
                }
            }

            return data;

        }

        public async Task<IEnumerable<UserData>> SelectByUserIdAsync(int userId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Selecting all user data for userId {userId}");
            }
                

            List<UserData> data = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserDatumByUserId",
                    userId);
                if (reader != null && reader.HasRows)
                {
                    data = new List<UserData>();
                    while (await reader.ReadAsync())
                    {
                        var userData = new UserData();
                        userData.PopulateModel(reader);
                        data.Add(userData);
                    }
                }
            }
            return data;

        }

        public async Task<UserData> InsertUpdateAsync(UserData data)
        {
            var id = await InsertUpdateInternal(
                data.Id,
                data.UserId,
                data.Key.ToEmptyIfNull().TrimToSize(255),
                data.Value.ToEmptyIfNull(),
                data.CreatedDate.ToDateIfNull(),
                data.CreatedUserId,
                data.ModifiedDate.ToDateIfNull(),
                data.ModifiedUserId);
            if (id > 0)
                return await SelectByIdAsync(id);
            return null;
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
     
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting user data id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteUserDatumById", id);
            }

            return success > 0 ? true : false;

        }
        
        public async Task<IPagedResults<UserData>> SelectAsync(params object[] inputParams)
        {

            PagedResults<UserData> output = null;
            using (var context = _dbContext)
            {

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserDatumPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<UserData>();
                    while (await reader.ReadAsync())
                    {
                        var data = new UserData();
                        data.PopulateModel(reader);
                        output.Data.Add(data);
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
            }

            return output;
            
        }
        
        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int userId,
            string key,
            string value,
            DateTime? createdDate,
            int createdUserId,
            DateTime? modifiedDate,
            int modifiedUserId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(id == 0
                    ? $"Inserting user data with key: {key}"
                    : $"Updating user data with id: {id}");
            }

            var output = 0;
            using (var context = _dbContext)
            {
                if (context == null)
                    return 0;
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUserDatum",
                    id,
                    userId,
                    key.ToEmptyIfNull().TrimToSize(255),
                    value.ToEmptyIfNull(),
                    createdDate.ToDateIfNull(),
                    createdUserId,
                    modifiedDate.ToDateIfNull(),
                    modifiedUserId,
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }

            return output;
        }
        
        #endregion

    }
}
