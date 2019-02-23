using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Cache.Abstractions;
using Plato.Search.Models;
using Plato.Search.Repositories;

namespace Plato.Search.Stores
{
    
    public class FullTextIndexStore : IFullTextIndexStore
    {

        private readonly IFullTextIndexRepository _fullTextIndexRepository;
        private readonly ICacheManager _cacheManager;

        public FullTextIndexStore(
            ICacheManager cacheManager, 
            IFullTextIndexRepository fullTextIndexRepository)
        {
            _cacheManager = cacheManager;
            _fullTextIndexRepository = fullTextIndexRepository;
        }

        public async Task<IEnumerable<FullTextIndex>> SelectIndexesAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _fullTextIndexRepository.SelectIndexesAsync());
        }

    }

}
