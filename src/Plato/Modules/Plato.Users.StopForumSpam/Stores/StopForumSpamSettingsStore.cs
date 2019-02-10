using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Stores.Abstract;
using Plato.Users.StopForumSpam.Models;

namespace Plato.Users.StopForumSpam.Stores
{
    
    public class StopForumSpamSettingsStore : IStopForumSpamSettingsStore<StopForumSpamSettings>
    {

        private const string SettingsKey = "StopForumSpamSettings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ILogger<StopForumSpamSettingsStore> _logger;
        private readonly ICacheManager _cacheManager;

        public StopForumSpamSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<StopForumSpamSettingsStore> logger,
            IMemoryCache memoryCache,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }

        public async Task<StopForumSpamSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<StopForumSpamSettings>(SettingsKey));
        }

        public async Task<StopForumSpamSettings> SaveAsync(StopForumSpamSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("StopForumSpam settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<StopForumSpamSettings>(SettingsKey, model);
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
