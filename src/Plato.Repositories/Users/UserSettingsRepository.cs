using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;
using Plato.Abstractions.Extensions;
using Plato.Abstractions.Data;
using Plato.Models.Users;
using Plato.Repositories.Settings;

namespace Plato.Repositories.Users
{
    public class UserSettingsRepository : IUserSettingsRepository<UserSetting>
    {

        #region Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<SettingRepository> _logger;

        #endregion

        #region "Constructor"

        public UserSettingsRepository(
            IDbContext dbContext,
            ILogger<SettingRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Deleting setting with id: {id}");
            
            throw new NotImplementedException();
        }

        public async Task<UserSetting> InsertUpdateAsync(UserSetting setting)
        {
            var id = await InsertUpdateInternal(
                setting.Id,
                setting.UserId,
                setting.Key.ToEmptyIfNull().TrimToSize(255),
                setting.Value.ToEmptyIfNull(),
                setting.CreatedDate.ToDateIfNull(),
                setting.CreatedUserId,
                setting.ModifiedDate.ToDateIfNull(),
                setting.ModifiedUserId);
            if (id > 0)
                return await SelectByIdAsync(id);
            return null;
        }

        public async Task<UserSetting> SelectByIdAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Selecting setting with id: {id}");

            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                    "SelectUserSettingById", id);
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        var setting = new UserSetting();
                        await reader.ReadAsync();
                        setting.PopulateModel(reader);
                        return setting;
                    }
                }

            }

            return null;

        }

        public async Task<IEnumerable<UserSetting>> SelectSettingsByUserId(int userId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Selecting all user settings for userId {userId}");

            List<UserSetting> settings = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserSettingsByUserId");
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        settings = new List<UserSetting>();
                        while (await reader.ReadAsync())
                        {
                            var setting = new UserSetting();
                            setting.PopulateModel(reader);
                            settings.Add(setting);
                        }
                    }
                }
            }
            return settings;

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
            int modifiedUserId
            )
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(id == 0
                    ? $"Inserting user settings with key: {key}"
                    : $"Updating user settings with id: {id}");
            }

            using (var context = _dbContext)
            {
                if (context == null)
                    return 0;
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUserSetting",
                    id,
                    userId,
                    key.ToEmptyIfNull().TrimToSize(255),
                    value.ToEmptyIfNull(),
                    createdDate.ToDateIfNull(),
                    createdUserId,
                    modifiedDate.ToDateIfNull(),
                    modifiedUserId);
            }

        }

        public Task<IPagedResults<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
