using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Models.Settings;
using Plato.Abstractions.Extensions;
using Plato.Abstractions.Settings;

namespace Plato.Repositories.Settings
{
    public class SettingsFactory : ISettingsFactory
    {

        #region "Private Variables"

        private ISettingRepository<Setting> _settingRepository;
         
        #endregion
        
        #region ""Constructor"

        public SettingsFactory(
            ISettingRepository<Setting> settingRepository)
        {
            _settingRepository = settingRepository;
        }

        #endregion

        #region "Implemention"

        public async Task<T> GetSettingsAsync<T>(string key)
        {
     
            var setting = await GetSettingFromRepository(key);
            if (setting != null)            
                return await setting.Value.DeserializeAsync<T>();                                 
        
            return await Task.FromResult<T>(default(T));

        }
        
        public async Task<T> UpdateSettingsAsync<T>(string key, ISettingValue value)
        {

            // get existing setting
            var existingSetting = await GetSettingFromRepository(key);

            // create new setting if not found
            if (existingSetting == null)
            {
                existingSetting = new Setting()
                {
                    Key = key
                };
            }

            // serilize object
            existingSetting.Value = value.Serialize();

            // update setting & return deserialized setting value
            var updatedSetting = await _settingRepository.InsertUpdate(existingSetting);
            if (updatedSetting != null)            
                return await GetSettingsAsync<T>(updatedSetting.Key);
               
            return await Task.FromResult<T>(default(T));

        }

        #endregion

        #region "Private Methods"


        private async Task<Setting> GetSettingFromRepository(string key)
        {
                     
            var availableSettings = await GetSettingsAsync();
            if (availableSettings != null)
                return availableSettings.Where(s => s.Key == key).FirstOrDefault();
        
            return await Task.FromResult<Setting>(default(Setting));

        }

        private async Task<IEnumerable<Setting>> GetSettingsAsync()
        {
            
            var output = new List<Setting>();
            IEnumerable<Setting> settings = await _settingRepository.SelectSettings();
            if (settings != null)
            {
                foreach (var setting in settings)                
                   output.Add(setting);  
            }

            return output;


        }
        #endregion



    }
}
