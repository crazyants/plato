using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Plato.Models.Roles;
using Plato.Repositories.Roles;
using Plato.Abstractions.Stores;
using Plato.Abstractions.Query;

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


        //public async Task<IEnumerable<Role>> GetAsync(int pageIndex, int pageSize, params object[] args)
        //{

        //    throw new NotImplementedException();

        //    //List<Role> roles;
        //    //if (!_memoryCache.TryGetValue(_key, out roles))
        //    //{
        //    //    roles = await _roleRepository(pageIndex, pageSize, null) as List<Role>;
        //    //    if (roles != null)
        //    //        _memoryCache.Set(_key, roles.ToList());
        //    //}

        //    //return roles;

        //}



        #endregion

        public Task<Role> CreateAsync(Role model)
        {
            throw new NotImplementedException();
        }

        public Task<Role> UpdateAsync(Role model)
        {
            throw new NotImplementedException();
        }

        public Task<Role> DeleteAsync(Role model)
        {
            throw new NotImplementedException();
        }

        public Task<Role> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IQuery QueryAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> SelectAsync<T>(params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

    }

}
