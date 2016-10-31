using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Models.Settings;
using Plato.Data;
using Microsoft.Extensions.Logging;
using System.Data;
using Plato.Abstractions.Extensions;
using System.Data.Common;
using Plato.Abstractions.Settings;

namespace Plato.Repositories.Settings
{
    public class SettingRepository : ISettingRepository<Setting>
    {

        #region Private Variables"

        private IDbContext _dbContext;
        ILogger<SettingRepository> _logger;
     
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

        public Task<Setting> DeleteAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<Setting> InsertUpdateAsync(Setting setting)
        {

            int id = await InsertUpdateInternal(
                setting.Id,          
                setting.SpaceId,
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

        public async Task<Setting> SelectByIdAsync(int Id)
        {

            Setting setting = new Setting();
            using (var context = _dbContext)
            {
                DbDataReader reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectSetting", Id);
               
                if (reader != null)
                {
                    await reader.ReadAsync();
                    setting.PopulateModel(reader);
                }
            }

            return setting;            

        }

        public async Task<IEnumerable<Setting>> SelectSettings()
        {

            List<Setting> settings = new List<Setting>();
            using (var context = _dbContext)
            {
                DbDataReader reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectSettings");

                if (reader != null)
                {
                    while (await reader.ReadAsync())
                    {
                        var setting = new Setting();
                        setting.PopulateModel(reader);
                        settings.Add(setting);
                    }

                }
            }

            return settings;

        }

        public async Task<IEnumerable<Setting>> SelectBySpaceId(int spaceId)
        {
            List<Setting> settings = new List<Setting>();
            using (var context = _dbContext)
            {
                IDataReader reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectSettingsBySpaceId", spaceId);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var setting = new Setting();
                        setting.PopulateModel(reader);
                        settings.Add(setting);
                    }

                }
            }

            return settings;
        }
        
        

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,    
            int spaceId,
            string key,
            string value,
            DateTime? createdDate,
            int createdUserId,
            DateTime? modifiedDate,
            int modifiedUserId
            )
        {
            
            using (var context = _dbContext)
            {
                return await context.ExecuteScalarAsync<int>(
                  CommandType.StoredProcedure,
                  "plato_sp_InsertUpdateSetting",
                    id,               
                    spaceId,
                    key.ToEmptyIfNull().TrimToSize(255),
                    value.ToEmptyIfNull(),
                    createdDate,
                    createdUserId,
                    modifiedDate,
                    modifiedUserId);
            }

      

        }

        public Task<IEnumerable<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
