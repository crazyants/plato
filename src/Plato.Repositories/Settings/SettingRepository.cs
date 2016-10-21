using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Models.Settings;
using Plato.Data;
using Microsoft.Extensions.Logging;
using System.Data;
using Plato.Abstractions.Extensions;
using System.Data.Common;

namespace Plato.Repositories.Settings
{
    public class SettingRepository : ISettingRepository<Setting>
    {

        #region Private Variables"

        private IDbContextt _dbContext;
        ILogger<SettingRepository> _logger;
     
        #endregion

        #region "Constructor"

        public SettingRepository(
            IDbContextt dbContext,
            ILogger<SettingRepository> logger)
        {

            _dbContext = dbContext;
            _logger = logger;

        }


        #endregion
        
        #region "Implementation"

        public Task<bool> Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<Setting> InsertUpdate(Setting setting)
        {

            int id = await InsertUpdateInternal(
                setting.Id,
                setting.SiteId,
                setting.SpaceId,
                setting.Key,
                setting.Value,
                setting.CreatedDate,
                setting.CreatedUserId,
                setting.ModifiedDate,
                setting.ModifiedUserId);

            if (id > 0)
                return await SelectById(id);

            return null;

        }

        public async Task<Setting> SelectById(int Id)
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

        public async Task<IEnumerable<Setting>> SelectBySiteId(int siteId)
        {

            List<Setting> settings = new List<Setting>();
            using (var context = _dbContext)
            {
                IDataReader reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectSettingsBySiteId", siteId);

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

        public Task<IEnumerable<Setting>> SelectPaged(int pageIndex, int pageSize, object options)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int Id,
            int SiteId,
            int SpaceId,
            string Key,
            string Value,
            DateTime? CreatedDate,
            int CreatedUserId,
            DateTime? ModifiedDate,
            int ModifiedUserId
            )
        {

            int id = 0;
            using (var context = _dbContext)
            {
                id = await context.ExecuteScalarAsync<int>(
                  CommandType.StoredProcedure,
                  "plato_sp_InsertUpdateSetting",
                    Id,
                    SiteId,
                    SpaceId,
                    Key.ToEmptyIfNull(),
                    Value.ToEmptyIfNull(),
                    CreatedDate,
                    CreatedUserId,
                    ModifiedDate,
                    ModifiedUserId);
            }

            return id;

        }

        #endregion


    }
}
