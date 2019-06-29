using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Facebook.Models;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Stores.Abstract;

namespace Plato.Facebook.Stores
{
    
    public class FacebookSettingsStore : IFacebookSettingsStore<FacebookSettings>
    {

        private const string SettingsKey = "FacebookSettings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ILogger<FacebookSettingsStore> _logger;
        private readonly ICacheManager _cacheManager;

        public FacebookSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<FacebookSettingsStore> logger,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }
        public async Task<FacebookSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<FacebookSettings>(SettingsKey));
        }

        public async Task<FacebookSettings> SaveAsync(FacebookSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Facebook settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<FacebookSettings>(SettingsKey, model);
            if (settings != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Facebook settings updated");
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
                    _logger.LogInformation("Facebook settings deleted");
                }
            }

            return result;

        }
    }
}
