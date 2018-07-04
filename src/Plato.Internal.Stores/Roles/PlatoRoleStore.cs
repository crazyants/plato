using System;
using System.Linq;
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

  
        #region "Constructor"
        
        private readonly IRoleRepository<Role> _roleRepository;
        private readonly ICacheDependency _cacheDependency;
        private readonly ILogger<PlatoRoleStore> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IDbQuery2 _dbQuery;
      
        public PlatoRoleStore(
            IRoleRepository<Role> roleRepository,
            ICacheDependency cacheDependency,
            ILogger<PlatoRoleStore> logger,
            IMemoryCache memoryCache,
            IDbQuery2 dbQuery)
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
                _cacheDependency.CancelToken(CacheKey.GetRolesCacheKey());
            }

            return newRole;
        }

        public async Task<Role> UpdateAsync(Role role)
        {

            var updatedRole = await _roleRepository.InsertUpdateAsync(role);

            if (updatedRole != null)
            {
                _cacheDependency.CancelToken(CacheKey.GetRolesCacheKey());
                _cacheDependency.CancelToken(CacheKey.GetRoleCacheKey(role.Id));
                _cacheDependency.CancelToken(CacheKey.GetRoleCacheKey(role.Name));
            }

            return updatedRole;

        }

        public async Task<bool> DeleteAsync(Role role)
        {

            var result = await _roleRepository.DeleteAsync(role.Id);

            if (result)
            {
                _cacheDependency.CancelToken(CacheKey.GetRolesCacheKey());
                _cacheDependency.CancelToken(CacheKey.GetRoleCacheKey(role.Id));
                _cacheDependency.CancelToken(CacheKey.GetRoleCacheKey(role.Name));
            }

            return result;

        }

        public async Task<Role> GetByIdAsync(int id)
        {
            var key = CacheKey.GetRoleCacheKey(id);
            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var role = await _roleRepository.SelectByIdAsync(id);
                if (role != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    }
                 
                    cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                }
                return role;
            });

        }

        public async Task<Role> GetByName(string name)
        {

            var key = CacheKey.GetRoleCacheKey(name);
            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var role = await _roleRepository.SelectByNameAsync(name);
                if (role != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    }

                    cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                }
                return role;
            });

        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            return await _roleRepository.SelectRoles();
        }

        public async Task<Role> GetByNormalizedName(string nameNormalized)
        {

            var key = CacheKey.GetRoleCacheKey(nameNormalized);
            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var role = await _roleRepository.SelectByNormalizedNameAsync(nameNormalized);
                if (role != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    }

                    cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                }
                return role;
            });
            
        }

        public IQuery QueryAsync()
        {
        throw new NotImplementedException();
        }

        public IQuery<Role> QueryAsync2()
        {
            var query = new RoleQuery(this);
            return _dbQuery.ConfigureQuery< Role>(query); ;
        }

        public async Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {

            var key = CacheKey.GetRolesCacheKey();

            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var roles = await _roleRepository.SelectAsync<T>(args);
                if (roles != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    }
                }
                cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                return roles;
            });
            
        }

        public async Task<IPagedResults<Role>> SelectAsync(params object[] args)
        {
            var key = CacheKey.GetRolesCacheKey();

            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var roles = await _roleRepository.SelectAsync<Role>(args);
                if (roles != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    }
                }
                cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                return roles;
            });

        }

        public async Task<IList<Role>> GetRolesByUserId(int userId)
        {
            
            var key = CacheKey.GetRolesByUserIdCacheKey(userId);
            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var role = await _roleRepository.SelectByUserIdAsync(userId);
                if (role != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    }

                    cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                }
                return role;
            });
           
        }

        public async Task<IEnumerable<string>> GetRoleNamesAsync()
        {
            var roles = await GetRolesAsync();
            return roles.Select(r => r.Name).OrderBy(r => r).ToList();
        }

        public async Task<IEnumerable<string>> GetRoleNamesByUserIdAsync(int userId)
        {
            var roles = await GetRolesByUserId(userId);
            return roles.Select(r => r.Name).OrderBy(r => r).ToList();
        }

        public async Task<IEnumerable<int>> GetRoleIdsByUserIdAsync(int userId)
        {
            var roles = await GetRolesByUserId(userId);
            return roles.Select(r => r.Id).OrderBy(r => r).ToList();
        }

        #endregion
        
    }
}