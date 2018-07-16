using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Https.Models;
using Plato.Internal.Stores.Abstract;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Https.Stores
{

    public interface IHttpsSettingsStore<T> : ISettingsStore<T> where T : class
    {

    }

    public class HttpsSettingsStore : IHttpsSettingsStore<HttpsSettings>
    {

        const string SettingsKey = "HttpsSettings";

        readonly IDictionaryStore _dictionaryStore;
        readonly ILogger<HttpsSettingsStore> _logger;
        readonly IMemoryCache _memoryCache;

        public HttpsSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<HttpsSettingsStore> logger,
            IMemoryCache memoryCache)
        {
            _dictionaryStore = dictionaryStore;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<HttpsSettings> GetAsync()
        {

            if (!_memoryCache.TryGetValue(SettingsKey, out HttpsSettings settings))
            {
                settings = await _dictionaryStore.GetAsync<HttpsSettings>(SettingsKey);
                if (settings != null)
                    _memoryCache.Set(SettingsKey, settings);
            }
            return settings;

        }

        public async Task<HttpsSettings> SaveAsync(HttpsSettings model)
        {

            var settings = await _dictionaryStore.UpdateAsync<HttpsSettings>(SettingsKey, model);
            if (settings != null)
            {
                _memoryCache.Set(SettingsKey, settings);
            }

            return settings;
        }

        public async Task<bool> DeleteAsync()
        {
            var result = await _dictionaryStore.DeleteAsync(SettingsKey);
            _memoryCache.Remove(SettingsKey);
            return result;
        }

    }

}
