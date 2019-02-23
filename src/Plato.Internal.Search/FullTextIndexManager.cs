using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Search.Abstractions;

namespace Plato.Internal.Search
{
    
    public class FullTextIndexManager : IFullTextIndexManager
    {

        private readonly IEnumerable<IFullTextIndexProvider> _providers;

        public FullTextIndexManager(IEnumerable<IFullTextIndexProvider> providers)
        {
            _providers = providers;
        }
        
        public async Task<IEnumerable<string>> CreateIndexesAsync()
        {
            var results = new List<string>();
            foreach (var provider in _providers)
            {
                results.AddRange(await provider.CreateIndexAsync());
            }

            return results;

        }

    }

}
