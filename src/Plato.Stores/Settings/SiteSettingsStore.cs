using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Plato.Abstractions.Settings;
using Plato.Abstractions.Stores;

namespace Plato.Stores.Settings
{
    public class SiteSettingsStore : ISiteSettingsStore
    {

        private readonly string _key = CacheKeys.SiteSettings.ToString();

        private readonly ISettingsFactory _settingsFactory;
        private readonly IMemoryCache _memoryCache;

        public SiteSettingsStore(
           ISettingsFactory settingsFactory,
           IMemoryCache memoryCache)
        {
            _settingsFactory = settingsFactory;
            _memoryCache = memoryCache;
        }
        
        public async Task<ISiteSettings> GetAsync()
        {
            if (!_memoryCache.TryGetValue(_key, out ISiteSettings siteSettings))
            {
                siteSettings = await _settingsFactory.GetSettingsAsync<SiteSettings>(_key);
                if (siteSettings != default(SiteSettings))
                    _memoryCache.Set(_key, siteSettings);
            }
            return siteSettings;
        }

        public async Task<ISiteSettings> SaveAsync(ISiteSettings siteSettings)
        {
            var settings = await _settingsFactory.UpdateSettingsAsync<SiteSettings>(_key, siteSettings);
            if (siteSettings != default(SiteSettings))
                _memoryCache.Set(_key, settings);
            return settings;
        }

        public Task<bool> DeleteAsync()
        {
            //var settings = await _settingsFactory<SiteSettings>(_key, siteSettings);
            _memoryCache.Remove(_key);
            return Task.FromResult(true);
        }

   

    }
}
