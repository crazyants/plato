using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Models.Schema;
using Plato.Internal.Repositories.Schema;
using Plato.Internal.Stores.Abstractions.Schema;

namespace Plato.Internal.Stores.Schema
{
    
    public class ConstraintStore : IConstraintStore
    {

        private readonly ICacheManager _cacheManager;
        private readonly IConstraintRepository _constraintRepository;

        public ConstraintStore(
            ICacheManager cacheManager,
            IConstraintRepository constraintRepository)
        {
            _cacheManager = cacheManager;
            _constraintRepository = constraintRepository;
        }

        public async Task<IEnumerable<DbConstraint>> SelectConstraintsAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _constraintRepository.SelectConstraintsAsync());
        }

    }

}
