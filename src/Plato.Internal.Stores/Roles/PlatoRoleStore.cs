using System.Linq;
using System.Collections.Generic;
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

        #region "Constructor"
        
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

        #endregion
        
        #region "Implementation"

        public async Task<Role> CreateAsync(Role role)
        {
            
            var newRole = await _roleRepository.InsertUpdateAsync(role);
            if (newRole != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Role with name {newRole.Name} created successfully");
                }
                ClearCache(role);
            }

            return newRole;
        }

        public async Task<Role> UpdateAsync(Role role)
        {

            var updatedRole = await _roleRepository.InsertUpdateAsync(role);

            if (updatedRole != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Role with name {updatedRole.Name} updated successfully");
                }
                ClearCache(role);
            }

            return updatedRole;

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
                ClearCache(role);
            }

            return result;

        }

        public async Task<Role> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _roleRepository.SelectByIdAsync(id));
        }

        public async Task<Role> GetByNameAsync(string name)
        {
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
            var token = _cacheManager.GetOrCreateToken(this.GetType(), nameNormalized);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _roleRepository.SelectByNormalizedNameAsync(nameNormalized));
        }

        public IQuery<Role> QueryAsync()
        {
            var query = new RoleQuery(this);
            return _dbQuery.ConfigureQuery< Role>(query); ;
        }
        
        public async Task<IPagedResults<Role>> SelectAsync(DbParam[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _roleRepository.SelectAsync(dbParams));
        }

        public async Task<IList<Role>> GetRolesByUserIdAsync(int userId)
        {
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

        void ClearCache(User user)
        {
            _cacheManager.CancelTokens(this.GetType(), UserId, user.Id);
        }
        
        void ClearCache(Role role)
        {
            _cacheManager.CancelTokens(this.GetType());
            _cacheManager.CancelTokens(this.GetType(), role.Id);
            _cacheManager.CancelTokens(this.GetType(), role.Name);
            _cacheManager.CancelTokens(this.GetType(), role.NormalizedName);
        }

        #endregion

    }
}