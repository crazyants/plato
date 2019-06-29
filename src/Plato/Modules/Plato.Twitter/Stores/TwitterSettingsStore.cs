using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Twitter.Models;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Stores.Abstract;

namespace Plato.Twitter.Stores
{
    
    public class TwitterSettingsStore : ITwitterSettingsStore<TwitterSettings>
    {

        private const string SettingsKey = "TwitterSettings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ILogger<TwitterSettingsStore> _logger;
        private readonly ICacheManager _cacheManager;

        public TwitterSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<TwitterSettingsStore> logger,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }
        public async Task<TwitterSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<TwitterSettings>(SettingsKey));
        }

        public async Task<TwitterSettings> SaveAsync(TwitterSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Twitter settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<TwitterSettings>(SettingsKey, model);
            if (settings != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Twitter settings updated");
                }

            }

            return settings;
        }

        public async Task<bool> DeleteAsync()
        {
            var result = await _dictionaryStore.DeleteAsync(SettingsKey);
            if (result)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Twitter settings deleted");
                }
            }

            return result;

        }
    }
}
