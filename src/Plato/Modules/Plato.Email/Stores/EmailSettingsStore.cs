using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Stores.Abstract;

namespace Plato.Email.Stores
{

    public class EmailSettingsStore : IEmailSettingsStore<EmailSettings>
    {

        private const string SettingsKey = "EmailSettings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ILogger<EmailSettingsStore> _logger;
        private readonly ICacheManager _cacheManager;

        public EmailSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<EmailSettingsStore> logger,
            IMemoryCache memoryCache,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }

        public async Task<EmailSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<EmailSettings>(SettingsKey));
        }

        public async Task<EmailSettings> SaveAsync(EmailSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Email settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<EmailSettings>(SettingsKey, model);
            if (settings != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Email settings updated");
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
                    _logger.LogInformation("Email settings deleted");
                }
            }

            return result;

        }

    }

}
