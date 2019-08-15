using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Users;
using Plato.Internal.Repositories.Roles;
using Plato.Internal.Stores.Abstractions.Roles;

namespace Plato.Internal.Stores.Roles
{
    public class PlatoRoleStore : IPlatoRoleStore
    {

        public const string UserId = "ByUserId";
        
        private readonly IRoleRepository<Role> _roleRepository;
        private readonly ILogger<PlatoRoleStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
      
        public PlatoRoleStore(
            IRoleRepository<Role> roleRepository,
            ILogger<PlatoRoleStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _roleRepository = roleRepository;
            _dbQuery = dbQuery;
            _cacheManager = cacheManager;
            _logger = logger;
        }
        
        public async Task<Role> CreateAsync(Role role)
        {
            
            var result = await _roleRepository.InsertUpdateAsync(role);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
        }

        public async Task<Role> UpdateAsync(Role role)
        {

            var result = await _roleRepository.InsertUpdateAsync(role);
            if (result != null)
            {
                CancelTokens(role);
            }

            return result;

        }

        public async Task<bool> DeleteAsync(Role role)
        {

            var result = await _roleRepository.DeleteAsync(role.Id);
            if (result)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Role with name {role.Name} was deleted successfully");
                }
                CancelTokens(role);
            }

            return result;

        }

        public async Task<Role> GetByIdAsync(int id)
        {

            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _roleRepository.SelectByIdAsync(id));

        }

        public async Task<Role> GetByNameAsync(string name)
        {

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), name);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _roleRepository.SelectByNameAsync(name));

        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _roleRepository.SelectRoles());
        }

        public async Task<Role> GetByNormalizedNameAsync(string nameNormalized)
        {

            if (string.IsNullOrEmpty(nameNormalized))
            {
                throw new ArgumentNullException(nameof(nameNormalized));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), nameNormalized);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _roleRepository.SelectByNormalizedNameAsync(nameNormalized));

        }

        public IQuery<Role> QueryAsync()
        {
            var query = new RoleQuery(this);
            return _dbQuery.ConfigureQuery< Role>(query); ;
        }
        
        public async Task<IPagedResults<Role>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _roleRepository.SelectAsync(dbParams));
        }

        public async Task<IList<Role>> GetRolesByUserIdAsync(int userId)
        {

            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), UserId, userId);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _roleRepository.SelectByUserIdAsync(userId));

        }

        public async Task<IEnumerable<string>> GetRoleNamesAsync()
        {
            var roles = await GetRolesAsync();
            return roles.Select(r => r.Name).OrderBy(r => r).ToList();
        }

        public async Task<IEnumerable<string>> GetRoleNamesByUserIdAsync(int userId)
        {
            var roles = await GetRolesByUserIdAsync(userId);
            return roles?.Select(r => r.Name).OrderBy(r => r).ToList();
        }

        public async Task<IEnumerable<int>> GetRoleIdsByUserIdAsync(int userId)
        {
            var roles = await GetRolesByUserIdAsync(userId);
            return roles?.Select(r => r.Id).OrderBy(r => r).ToList();
        }
        
        public void CancelTokens(Role model = null)
        {

            _cacheManager.CancelTokens(this.GetType());

            if (model != null)
            {
                _cacheManager.CancelTokens(this.GetType(), model.Id);
                _cacheManager.CancelTokens(this.GetType(), model.Name);
                _cacheManager.CancelTokens(this.GetType(), model.NormalizedName);
            }
          
        }

        void ClearCache(User user)
        {
            _cacheManager.CancelTokens(this.GetType(), UserId, user.Id);
        }
        
    }

}