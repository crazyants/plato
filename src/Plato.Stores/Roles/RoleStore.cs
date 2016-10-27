using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Plato.Models.Roles;
using Plato.Repositories.Roles;
using Plato.Abstractions.Stores;

namespace Plato.Stores.Roles
{
    public class RoleStore : IRoleStore
    {


        #region "Private Variables"

        private readonly string _key = CacheKeys.Roles.ToString();

        private readonly IRoleRepository<Role> _roleRepository;
        private readonly IMemoryCache _memoryCache;

        #endregion

        #region "Constructor"

        public RoleStore(
           IRoleRepository<Role> roleRepository,
           IMemoryCache memoryCache)
        {
            _roleRepository = roleRepository;
            _memoryCache = memoryCache;
        }

        #endregion

        #region "Implementation"


        public async Task<IEnumerable<Role>> GetAsync(int pageIndex, int pageSize, params object[] args)
        {

            List<Role> roles;
            if (!_memoryCache.TryGetValue(_key, out roles))
            {
                roles = await _roleRepository.SelectPagedAsync(pageIndex, pageSize, null) as List<Role>;
                if (roles != null)
                    _memoryCache.Set(_key, roles.ToList());
            }

            return roles;

        }

        public Task<Role> GetAsync(Role model)
        {
            throw new NotImplementedException();
        }

        public Task<Role> SaveAsync(Role model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Role model)
        {
            throw new NotImplementedException();
        }


        #endregion





    }
}
