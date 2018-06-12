using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Models.Settings;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Abstractions.Settings;

namespace Plato.Internal.Repositories.Settings
{
    public class SettingsFactory : ISettingsFactory
    {

        #region "Private Variables"

        private readonly ISettingRepository<Setting> _settingRepository;
         
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
            return default(T);
        }
        
        public async Task<T> UpdateSettingsAsync<T>(string key, ISettingValue value)
        {

            // Get or create setting
            var existingSetting = await GetSettingFromRepository(key)
                                  ?? new Setting()
                                  {
                                      Key = key
                                  };

            // Serilize value
            existingSetting.Value = value.Serialize();

            // update setting & return updated typed settings 
            var updatedSetting = await _settingRepository.InsertUpdateAsync(existingSetting);
            if (updatedSetting != null)
            {
                return await GetSettingsAsync<T>(updatedSetting.Key);
            }         
                
            return default(T);

        }

        #endregion

        #region "Private Methods"
        
        private async Task<Setting> GetSettingFromRepository(string key)
        {
            var availableSettings = await GetSettingsAsync();
            return availableSettings?.FirstOrDefault(s => s.Key == key);
        }

        private async Task<IEnumerable<Setting>> GetSettingsAsync()
        {
            var output = new List<Setting>();
            var settings = await _settingRepository.SelectSettings();
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
