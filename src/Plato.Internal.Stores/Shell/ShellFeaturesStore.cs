using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Plato.Internal.Models.Shell;
using Plato.Internal.Stores.Abstract;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Internal.Stores.Shell
{
    public class ShellFeaturesStore : IShellFeaturesStore
    {

        private readonly string _key = CacheKeys.ShellFeatures.ToString();

        private readonly IDictionaryStore _dictionaryStore;
        private readonly IMemoryCache _memoryCache;

        public ShellFeaturesStore(
            IDictionaryStore dictionaryStore,
            IMemoryCache memoryCache)
        {
            _dictionaryStore = dictionaryStore;
            _memoryCache = memoryCache;
        }

        public async Task<IShellFeatures> GetAsync()
        {
            if (!_memoryCache.TryGetValue(_key, out ShellFeatures shellFeatures))
            {
                shellFeatures = await _dictionaryStore.GetAsync<ShellFeatures>(_key);
                if (shellFeatures != null)
                    _memoryCache.Set(_key, shellFeatures);
            }

            return shellFeatures;
        }

        public async Task<IShellFeatures> SaveAsync(IShellFeatures shellFeatures)
        {
            var features = await _dictionaryStore.UpdateAsync<IShellFeatures>(_key, shellFeatures);
            if (features != null)
            {
                // Update cache
                _memoryCache.Set(_key, features);
            }

            return features;
        }

        public async Task<bool> DeleteAsync()
        {
            var result = await _dictionaryStore.DeleteAsync(_key);
            _memoryCache.Remove(_key);
            return result;
        }

    }

}
