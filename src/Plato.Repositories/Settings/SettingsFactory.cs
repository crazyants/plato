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

        public T GetSettings<T>(string key)
        {
            throw new NotImplementedException();
        }

        public T InsertSettings<T>(string key, ISettingValue value)
        {
            throw new NotImplementedException();
        }

        public T UpdateSettings<T>(string key, ISettingValue value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Private Methods"

        private async Task AvailableSettings()
        {
         

        }


        private async Task<Dictionary<string, string>> GetAvailableSettings()
        {


            var output = new Dictionary<string, string>();
            IEnumerable<Setting> settings = await _settingRepository.SelectSettings();
            if (settings != null)
            {
                foreach (var setting in settings)
                {
                    foreach (KeyValuePair<string, string> kvp in setting.Settings)
                    {
                        if (!output.ContainsKey(kvp.Key))
                            output.Add(kvp.Key, kvp.Value);
                    }
                }

            }

            return output;


        }
        #endregion



    }
}
