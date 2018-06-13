using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Repositories.Abstract;
using Plato.Internal.Stores.Abstract;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Internal.Stores.Settings
{
    public class SiteSettingsStore : ISiteSettingsStore
    {

        private readonly string _key = CacheKeys.SiteSettings.ToString();

        private readonly IDictionaryStore _dictionaryStore;
        private readonly IMemoryCache _memoryCache;

        public SiteSettingsStore(
            IDictionaryStore dictionaryStore,
            IMemoryCache memoryCache)
        {
            _dictionaryStore = dictionaryStore;
            _memoryCache = memoryCache;
        }

        public async Task<ISiteSettings> GetAsync()
        {
            if (!_memoryCache.TryGetValue(_key, out SiteSettings siteSettings))
            {
                siteSettings = await _dictionaryStore.GetAsync<SiteSettings>(_key);
                if (siteSettings != null)
                    _memoryCache.Set(_key, siteSettings);
            }

            return siteSettings;
        }

        public async Task<ISiteSettings> SaveAsync(ISiteSettings siteSettings)
        {
            var settings = await _dictionaryStore.UpdateAsync<SiteSettings>(_key, siteSettings);
            if (settings != null)
            {
                // Update cache
                _memoryCache.Set(_key, settings);
            }

            return settings;
        }

        public async Task<bool> DeleteAsync()
        {
            var result =  await _dictionaryStore.DeleteAsync(_key);
            _memoryCache.Remove(_key);
            return result;
        }

    }
}
