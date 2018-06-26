using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Shell;
using Plato.Internal.Stores.Abstract;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Internal.Stores.Shell
{
    public class ShellDescriptorStore : IShellDescriptorStore
    {

        private readonly string _key = CacheKeys.ShellDescriptor.ToString();

        private readonly IDictionaryStore _dictionaryStore;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ShellDescriptorStore> _logger;

        public ShellDescriptorStore(
            IDictionaryStore dictionaryStore,
            IMemoryCache memoryCache, 
            ILogger<ShellDescriptorStore> logger)
        {
            _dictionaryStore = dictionaryStore;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<IShellDescriptor> GetAsync()
        {
            if (!_memoryCache.TryGetValue(_key, out IShellDescriptor descriptor))
            {
                descriptor = await _dictionaryStore.GetAsync<ShellDescriptor>(_key);
                if (descriptor != null)
                    _memoryCache.Set(_key, descriptor);
            }

            return descriptor;
        }

        public async Task<IShellDescriptor> SaveAsync(IShellDescriptor shellDescriptor)
        {

            // Ensure we have a distinct list of features before calculating any id
            var distinctDictionary = new ConcurrentDictionary<string, ShellModule>();
            foreach (var module in shellDescriptor.Modules)
            {
                if (!distinctDictionary.ContainsKey(module.ModuleId))
                {
                    distinctDictionary.TryAdd(module.ModuleId, module);
                }
            }

            // Ensure each distinct feature has a unique Id
            var distinctModules = distinctDictionary.Values.ToList();
            for (var i = 0; i < distinctModules.Count; i++)
            {
                if (distinctModules[i].Id == 0)
                {
                    distinctModules[i].Id = i + 1;
                }
            }

            shellDescriptor.Modules = distinctDictionary.Values.ToList();

            var descriptor = await _dictionaryStore.UpdateAsync<ShellDescriptor>(_key, shellDescriptor);
            if (descriptor != null)
            {
                _memoryCache.Set(_key, descriptor);
            }

            return descriptor;
        }

        public async Task<bool> DeleteAsync()
        {
            var result = await _dictionaryStore.DeleteAsync(_key);
            _memoryCache.Remove(_key);
            return result;
        }

    }

}
