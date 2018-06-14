using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
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

        public ShellDescriptorStore(
            IDictionaryStore dictionaryStore,
            IMemoryCache memoryCache)
        {
            _dictionaryStore = dictionaryStore;
            _memoryCache = memoryCache;
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
