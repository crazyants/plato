using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Email.Models;
using Plato.Internal.Stores.Abstract;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Email.Stores
{

    public interface IEmailSettingsStore<T> : ISettingsStore<T> where T : class
    {

    }

    public class EmailSettingsStore : IEmailSettingsStore<EmailSettings>
    {

        const string _key = "EmailSettings";

        readonly IDictionaryStore _dictionaryStore;
        readonly ILogger<EmailSettingsStore> _logger;
        readonly IMemoryCache _memoryCache;

        public EmailSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<EmailSettingsStore> logger,
            IMemoryCache memoryCache)
        {
            _dictionaryStore = dictionaryStore;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<EmailSettings> GetAsync()
        {

            if (!_memoryCache.TryGetValue(_key, out EmailSettings settings))
            {
                settings = await _dictionaryStore.GetAsync<EmailSettings>(_key);
                if (settings != null)
                    _memoryCache.Set(_key, settings);
            }
            return settings;

        }

        public async Task<EmailSettings> SaveAsync(EmailSettings model)
        {

            var settings = await _dictionaryStore.UpdateAsync<EmailSettings>(_key, model);
            if (settings != null)
            {
                _memoryCache.Set(_key, settings);
            }

            return settings;
        }

        public async Task<bool> DeleteAsync()
        {
            var result = await _dictionaryStore.DeleteAsync(_key);
            _memoryCache.Remove(_key);
            return result;
        }

    }

}
