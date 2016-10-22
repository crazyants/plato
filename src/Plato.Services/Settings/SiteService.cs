using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Repositories.Settings;
using Plato.Models.Settings;
using Plato.Abstractions.Settings;
using Microsoft.Extensions.Caching.Memory;

namespace Plato.Services.Settings
{
    public class SiteService : ISiteService
    {

        private string SiteCacheKey = "SiteSettings";
        private IDictionary<string, string> _settings;

        ISettingRepository<Setting> _settingRepository;
        IMemoryCache _memoryCache;

        public SiteService(
           ISettingRepository<Setting> settingRepository,
           IMemoryCache memoryCache)
        {
            _settingRepository = settingRepository;
            _memoryCache = memoryCache;
        }

        public async Task<ISiteSettings> GetSiteSettingsAsync()
        {

            throw new NotImplementedException();
         
            ISiteSettings site;

            if (!_memoryCache.TryGetValue(SiteCacheKey, out site))
            {

                _settings = new Dictionary<string, string>();
                IEnumerable<Setting> settings = await _settingRepository.SelectSettings();
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

                _memoryCache.Set(SiteCacheKey, site);


            }



        }

         

        public Task UpdateSiteSettingsAsync(ISiteSettings site)
        {
            throw new NotImplementedException();
        }

    }
}
