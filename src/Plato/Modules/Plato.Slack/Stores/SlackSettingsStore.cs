using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Stores.Abstract;
using Plato.Slack.Models;

namespace Plato.Slack.Stores
{
    
    public class SlackSettingsStore : ISlackSettingsStore<SlackSettings>
    {

        private const string SettingsKey = "SlackSettings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ILogger<SlackSettingsStore> _logger;
        private readonly ICacheManager _cacheManager;

        public SlackSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<SlackSettingsStore> logger,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }
        public async Task<SlackSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<SlackSettings>(SettingsKey));
        }

        public async Task<SlackSettings> SaveAsync(SlackSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Slack settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<SlackSettings>(SettingsKey, model);
            if (settings != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Slack settings updated");
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
                    _logger.LogInformation("Slack settings deleted");
                }
            }

            return result;

        }
    }
}
