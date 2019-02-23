using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Cache.Abstractions;
using Plato.Search.Models;
using Plato.Search.Repositories;

namespace Plato.Search.Stores
{
    

    public class FullTextCatalogStore : IFullTextCatalogStore
    {

        private readonly IFullTextCatalogRepository _fullTextCatalogRepository;
        private readonly ICacheManager _cacheManager;

        public FullTextCatalogStore(
            ICacheManager cacheManager, 
            IFullTextCatalogRepository fullTextCatalogRepository)
        {
            _cacheManager = cacheManager;
            _fullTextCatalogRepository = fullTextCatalogRepository;
        }

        public async Task<IEnumerable<FullTextCatalog>> SelectCatalogsAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _fullTextCatalogRepository.SelectCatalogsAsync());
        }
    }
}
