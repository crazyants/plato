using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Roles;
using Plato.Internal.Repositories.Roles;
using Plato.Internal.Stores.Abstractions.Roles;

namespace Plato.Internal.Stores.Roles
{
    public class PlatoRoleStore : IPlatoRoleStore
    {

        #region "Private Variables"

        private readonly string _key = CacheKeys.Roles.ToString();

        #endregion

        #region "Constructor"
        
        private readonly IRoleRepository<Role> _roleRepository;
        private readonly ICacheDependency _cacheDependency;
        private readonly ILogger<PlatoRoleStore> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IDbQuery _dbQuery;
      
        public PlatoRoleStore(
            IRoleRepository<Role> roleRepository,
            ICacheDependency cacheDependency,
            ILogger<PlatoRoleStore> logger,
            IMemoryCache memoryCache,
            IDbQuery dbQuery)
        {
            _roleRepository = roleRepository;
            _cacheDependency = cacheDependency;
            _memoryCache = memoryCache;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #endregion
        
        #region "Implementation"

        public async Task<Role> CreateAsync(Role model)
        {
            
            var newRole = await _roleRepository.InsertUpdateAsync(model);
            if (newRole != null)
            {
                _cacheDependency.CancelToken(_key);
            }

            return newRole;
        }

        public async Task<Role> UpdateAsync(Role role)
        {

            var updatedRole = await _roleRepository.InsertUpdateAsync(role);

            if (updatedRole != null)
            {
                _cacheDependency.CancelToken(_key);
                _cacheDependency.CancelToken(GetRoleEntryCacheKey(role.Id));
                _cacheDependency.CancelToken(GetRoleEntryCacheKey(role.Name));
            }

            return updatedRole;

        }

        public async Task<bool> DeleteAsync(Role role)
        {

            var result = await _roleRepository.DeleteAsync(role.Id);

            if (result)
            {
                _cacheDependency.CancelToken(_key);
                _cacheDependency.CancelToken(GetRoleEntryCacheKey(role.Id));
                _cacheDependency.CancelToken(GetRoleEntryCacheKey(role.Name));
            }

            return result;

        }

        public async Task<Role> GetByIdAsync(int id)
        {
            var key = GetRoleEntryCacheKey(id);
            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var role = await _roleRepository.SelectByIdAsync(id);
                if (role != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, _key);
                    }
                 
                    cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                }
                return role;
            });

        }

        public async Task<Role> GetByName(string name)
        {

            var key = GetRoleEntryCacheKey(name);
            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var role = await _roleRepository.SelectByNameAsync(name);
                if (role != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, _key);
                    }

                    cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                }
                return role;
            });

        }

        public async Task<Role> GetByNormalizedName(string nameNormalized)
        {

            var key = GetRoleEntryCacheKey(nameNormalized);
            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var role = await _roleRepository.SelectByNormalizedNameAsync(nameNormalized);
                if (role != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, _key);
                    }

                    cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                }
                return role;
            });
            
        }

        public IQuery QueryAsync()
        {
            var query = new RoleQuery(this);
            return _dbQuery.ConfigureQuery(query); ;
        }

        public async Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {

            return await _memoryCache.GetOrCreateAsync(_key, async (cacheEntry) =>
            {
                var roles = await _roleRepository.SelectAsync<T>(args);
                if (roles != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, _key);
                    }
                }
                cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(_key));
                return roles;
            });
            
        }

        public async Task<IList<Role>> GetRolesByUserId(int userId)
        {
            
            var key = GetRoleByUserIdEntryCacheKey(userId);
            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var role = await _roleRepository.SelectByUserIdAsync(userId);
                if (role != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, _key);
                    }

                    cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                }
                return role;
            });
           
        }
        
        #endregion

        #region "Private Methods"
        
        private string GetRoleEntryCacheKey(int roleId)
        {
            return $"{_key}_{roleId.ToString()}";
        }

        private string GetRoleEntryCacheKey(string roleName)
        {
            return $"{_key}_{roleName.ToString()}";
        }

        public string GetRoleByUserIdEntryCacheKey(int userId)
        {
            return $"{_key}_UserRoles_{userId.ToString()}";
        }

        #endregion


    }
}