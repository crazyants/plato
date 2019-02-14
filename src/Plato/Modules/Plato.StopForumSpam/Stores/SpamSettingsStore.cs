using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Stores.Abstract;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.Stores
{
    
    public class SpamSettingsStore : ISpamSettingsStore<SpamSettings>
    {

        private const string SettingsKey = "StopForumSpamSettings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ILogger<SpamSettingsStore> _logger;
        private readonly ICacheManager _cacheManager;

        public SpamSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<SpamSettingsStore> logger,
            IMemoryCache memoryCache,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }

        public async Task<SpamSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<SpamSettings>(SettingsKey));
        }

        public async Task<SpamSettings> SaveAsync(SpamSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("StopForumSpam settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<SpamSettings>(SettingsKey, model);
            if (settings != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("StopForumSpam settings updated");
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
                    _logger.LogInformation("StopForumSpam settings deleted");
                }
            }

            return result;

        }

    }

}
