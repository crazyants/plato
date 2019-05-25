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
            
            UserData userData = null;
            using (var context = _dbContext)
            {
                userData = await context.ExecuteReaderAsync2(
                    CommandType.StoredProcedure,
                    "SelectUserDatumById",
                    async reader =>
                    {
                        if (reader != null && reader.HasRows)
                        {
                            userData = new UserData();
                            await reader.ReadAsync();
                            userData.PopulateModel(reader);
                        }

                        return userData;
                    }, new []
                    {
                        new DbParam("Id", DbType.Int32, id),
                    });
            

            }

            return userData;

        }

        public async Task<UserData> SelectByKeyAndUserIdAsync(string key, int userId)
        {

            UserData data = null;
            using (var context = _dbContext)
            {
                data = await context.ExecuteReaderAsync2(
                    CommandType.StoredProcedure,
                    "SelectUserDatumByKeyAndUserId",
                    async reader =>
                    {
                        if (reader != null && reader.HasRows)
                        {
                            data = new UserData();
                            await reader.ReadAsync();
                            data.PopulateModel(reader);
                        }

                        return data;
                    }, new []
                    {
                        new DbParam("Key", DbType.String, 255, key),
                        new DbParam("UserId", DbType.Int32, userId),
                    });

            }

            return data;

        }

        public async Task<IEnumerable<UserData>> SelectByUserIdAsync(int userId)
        {

            IList<UserData> data = null;
            using (var context = _dbContext)
            {
                data = await context.ExecuteReaderAsync2<IList<UserData>>(
                    CommandType.StoredProcedure,
                    "SelectUserDatumByUserId",
                    async reader =>
                    {
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

                        return data;
                    }, new[]
                    {
                        new DbParam("UserId", DbType.Int32, userId)
                    });
             
            }
            return data;

        }

        public async Task<UserData> InsertUpdateAsync(UserData data)
        {
            var id = await InsertUpdateInternal(
                data.Id,
                data.UserId,
                data.Key,
                data.Value,
                data.CreatedUserId,
                data.CreatedDate,
                data.ModifiedUserId,
                data.ModifiedDate);
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
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteUserDatumById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }
        
        public async Task<IPagedResults<UserData>> SelectAsync(DbParam[] dbParams)
        {

            IPagedResults<UserData> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync2<IPagedResults<UserData>>(
                    CommandType.StoredProcedure,
                    "SelectUserDatumPaged",
                    async reader =>
                    {
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

                        return output;
                    },
                    dbParams
                );

              
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
            int createdUserId,
            DateTimeOffset? createdDate,
            int modifiedUserId,
            DateTimeOffset? modifiedDate)
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
                output = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUserDatum",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("UserId", DbType.Int32, userId),
                        new DbParam("Key", DbType.String, 255, key),
                        new DbParam("Value", DbType.String, value),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output)
                    });
            }

            return output;
        }
        
        #endregion

    }
}
