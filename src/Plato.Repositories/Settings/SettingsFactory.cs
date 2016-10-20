using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Models.Settings;
using Plato.Abstractions.Extensions;

namespace Plato.Repositories.Settings
{
    public class SettingsFactory : ISettingsFactory
    {

        #region "Private Variables"

        private ISettingRepository<Setting> _settingRepository;

        private IDictionary<string, string> _settings;

        #endregion


        #region ""Constructor"

        public SettingsFactory(
            ISettingRepository<Setting> settingRepository)
        {
            _settingRepository = settingRepository;
        }

        #endregion

        #region "Implementation"

        public string TryGetValue(string key)
        {
            string value = null;
            bool ok = _settings.TryGetValue(key, out value);
            if (ok)
                return value;

            return null;
        }

        public IDictionary<string, string> Settings
        {
            get { return _settings; }
        }

        public async Task<ISettingsFactory> SelectById(int Id)
        {

            var setting = await _settingRepository.SelectById(Id);
            if (setting != null)       
                _settings = setting.Value.JsonToDictionary();

            return this;
        }

        public async Task<ISettingsFactory> SelectBySiteId(int SiteId)
        {

            _settings = new Dictionary<string, string>();
            IEnumerable<Setting> settings = await _settingRepository.SelectBySiteId(SiteId);
            if (settings != null)
            {
                foreach (var setting in settings)
                {
                    foreach (KeyValuePair<string, string> kvp in setting.Settings)
                    {
                        if (!_settings.ContainsKey(kvp.Key))
                            _settings.Add(kvp.Key, kvp.Value);
                    }
                }

            }

            return this;   

        }

        public async Task<ISettingsFactory> SelectBySpaceId(int SpaceId)
        {

            _settings = new Dictionary<string, string>();
            IEnumerable<Setting> settings = await _settingRepository.SelectBySpaceId(SpaceId);
            if (settings != null)
            {
                foreach (var setting in settings)
                {
                    foreach (KeyValuePair<string, string> kvp in setting.Settings)
                    {
                        if (!_settings.ContainsKey(kvp.Key))
                            _settings.Add(kvp.Key, kvp.Value);
                    }
                }

            }

            return this;


        }


        #endregion
          

    }
}
