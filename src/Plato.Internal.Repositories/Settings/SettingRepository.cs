using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Settings;
using Microsoft.Extensions.Logging;
using System.Data;
using Plato.Abstractions.Extensions;
using Plato.Abstractions.Data;

namespace Plato.Internal.Repositories.Settings
{
    public class SettingRepository : ISettingRepository<Setting>
    {

        #region Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<SettingRepository> _logger;
     
        #endregion

        #region "Constructor"

        public SettingRepository(
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

        public async Task<Setting> InsertUpdateAsync(Setting setting)
        {
            var id = await InsertUpdateInternal(
                setting.Id,    
                setting.Key,
                setting.Value,
                setting.CreatedDate,
                setting.CreatedUserId,
                setting.ModifiedDate,
                setting.ModifiedUserId);
            if (id > 0)
                return await SelectByIdAsync(id);
            return null;
        }

        public async Task<Setting> SelectByIdAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Selecting setting with id: {id}");
            
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                    "SelectSettingById", id);
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        var setting = new Setting();
                        await reader.ReadAsync();
                        setting.PopulateModel(reader);
                        return setting;
                    }
                }
          
            }

            return null;
            
        }

        public async Task<IEnumerable<Setting>> SelectSettings()
        {

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Selecting all settings");

            List<Setting> settings = null;
            // database context may not be configured.
            // For example during set-up
            if (_dbContext != null)
            {
                using (var context = _dbContext)
                {
                    var reader = await context.ExecuteReaderAsync(
                        CommandType.StoredProcedure,
                        "SelectSettings");
                    if (reader != null)
                    {
                        if (reader.HasRows)
                        {
                            settings = new List<Setting>();
                            while (await reader.ReadAsync())
                            {
                                var setting = new Setting();
                                setting.PopulateModel(reader);
                                settings.Add(setting);
                            }
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
                    ? $"Inserting settings with key: {key}"
                    : $"Updating settings with id: {id}");
            }
              
            using (var context = _dbContext)
            {
                if (context == null)
                    return 0;
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateSetting",
                    id,
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
