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

        private string _key = "SiteSettings";
    
        ISettingsFactory _settingsFactory;
        IMemoryCache _memoryCache;

        public SiteService(
           ISettingsFactory settingsFactory,
           IMemoryCache memoryCache)
        {
            _settingsFactory =settingsFactory;
            _memoryCache = memoryCache;
        }

        public async Task<ISiteSettings> GetSiteSettingsAsync()
        {
                    
            ISiteSettings siteSettings;
            if (!_memoryCache.TryGetValue(_key, out siteSettings))
            {
                siteSettings = await _settingsFactory.GetSettingsAsync<SiteSettings>(_key);
                _memoryCache.Set(_key, siteSettings);
            }
            
            return siteSettings;

        }
    
        public async Task<ISiteSettings> UpdateSiteSettingsAsync(ISiteSettings siteSettings)
        {
            var settings = await _settingsFactory.UpdateSettingsAsync<SiteSettings>(_key, siteSettings);
            _memoryCache.Set(_key, settings);
            return settings;
        }

    }
}
