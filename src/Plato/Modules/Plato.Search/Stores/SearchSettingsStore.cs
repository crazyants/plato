using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Stores.Abstract;
using Plato.Search.Models;

namespace Plato.Search.Stores
{
    
    public class SearchSettingsStore : ISearchSettingsStore<SearchSettings>
    {

        private const string SettingsKey = "SearchSettings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ILogger<SearchSettingsStore> _logger;
        private readonly ICacheManager _cacheManager;

        public SearchSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<SearchSettingsStore> logger,
            IMemoryCache memoryCache,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }

        public async Task<SearchSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<SearchSettings>(SettingsKey));
        }

        public async Task<SearchSettings> SaveAsync(SearchSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Email settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<SearchSettings>(SettingsKey, model);
            if (settings != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Search settings updated");
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
                    _logger.LogInformation("Search settings deleted");
                }
            }

            return result;

        }

    }

}
