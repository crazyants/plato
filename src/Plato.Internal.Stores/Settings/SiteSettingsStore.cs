using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Routing;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Stores.Abstract;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Stores.Settings
{
    public class SiteSettingsStore : ISiteSettingsStore
    {

        private const string Key = "SiteSettings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<SiteSettingsStore> _logger;
        private readonly IKeyGenerator _keyGenerator;

        public SiteSettingsStore(
            IDictionaryStore dictionaryStore,
            IMemoryCache memoryCache,
            ICacheManager cacheManager,
            ILogger<SiteSettingsStore> logger,
            IKeyGenerator keyGenerator)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
            _keyGenerator = keyGenerator;
        }

        public async Task<ISiteSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), Key);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<SiteSettings>(Key));
        }

        public async Task<ISiteSettings> SaveAsync(ISiteSettings siteSettings)
        {

            // Automatically generate an API key if one is not supplied
            if (String.IsNullOrWhiteSpace(siteSettings.ApiKey))
            {
                siteSettings.ApiKey = _keyGenerator.GenerateKey();
            }

            // Use default homepage route if a default route is not explictly specified
            if (siteSettings.HomeRoute == null)
            {
                siteSettings.HomeRoute = new DefaultHomePageRoute();
            }
            
            // Update settings
            var settings = await _dictionaryStore.UpdateAsync<SiteSettings>(Key, siteSettings);
            if (settings != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Settings for site '{settings.SiteName}' updated successfully");
                }
                // Expire cache
                _cacheManager.CancelTokens(this.GetType(), Key);
            }

            return settings;
        }

        public async Task<bool> DeleteAsync()
        {
            var result =  await _dictionaryStore.DeleteAsync(Key);
            if (result)
            {
                // Expire cache
                _cacheManager.CancelTokens(this.GetType(), Key);
            }
            
            return result;
        }
        
    }
}
