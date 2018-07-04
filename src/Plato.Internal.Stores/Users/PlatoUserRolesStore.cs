using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Repositories.Users;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Internal.Stores.Users
{
    public class PlatoUserRolesStore : IPlatoUserRoleStore<UserRole>
    {

        private const string  Key = "UserRoles";

        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly IUserRolesRepository<UserRole> _userRolesRepository;
        private readonly ILogger<PlatoUserRolesStore> _logger;
        private readonly ICacheDependency _cacheDependency;
        private readonly IMemoryCache _memoryCache;

        public PlatoUserRolesStore(
            IPlatoRoleStore platoRoleStore,
            IUserRolesRepository<UserRole> userRolesRepository,
            ILogger<PlatoUserRolesStore> logger,
            ICacheDependency cacheDependency,
            IMemoryCache memoryCache)
        {
            _platoRoleStore = platoRoleStore;
            _userRolesRepository = userRolesRepository;
            _logger = logger;
            _cacheDependency = cacheDependency;
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<UserRole>> AddtUserRolesAsync(int userId, IEnumerable<string> roleNames)
        {
            return await _userRolesRepository.InsertUserRolesAsync(userId, roleNames);
        }

        public async Task<IEnumerable<UserRole>> AddUserRolesAsync(int userId, IEnumerable<int> roleIds)
        {
            return await _userRolesRepository.InsertUserRolesAsync(userId, roleIds);
        }

        public async Task<UserRole> CreateAsync(UserRole model)
        {

            var userRole = await _userRolesRepository.InsertUpdateAsync(model);
            if (userRole != null)
            {
                _cacheDependency.CancelToken(CacheKey.GetRolesByUserIdCacheKey(model.UserId));
            }

            return userRole;

        }
        
        public async Task<UserRole> UpdateAsync(UserRole model)
        {
            var userRole = await _userRolesRepository.InsertUpdateAsync(model);
            if (userRole != null)
            {
                _cacheDependency.CancelToken(CacheKey.GetRolesByUserIdCacheKey(model.UserId));
            }

            return userRole;
        }

        public async Task<bool> DeleteAsync(UserRole model)
        {

            var success = await _userRolesRepository.DeleteAsync(model.Id);
            if (success)
            {
                _cacheDependency.CancelToken(CacheKey.GetRolesByUserIdCacheKey(model.UserId));
            }

            return success;

        }

        public async Task<bool> DeletetUserRole(int userId, int roleId)
        {
            var success = await _userRolesRepository.DeletetUserRole(userId, roleId);
            if (success)
            {
                _cacheDependency.CancelToken(CacheKey.GetRolesByUserIdCacheKey(userId));
            }

            return success;
        }
        
        public async Task<bool> DeletetUserRolesAsync(int userId)
        {
            var success = await _userRolesRepository.DeletetUserRolesAsync(userId);
            if (success)
            {
                _cacheDependency.CancelToken(CacheKey.GetRolesByUserIdCacheKey(userId));
            }

            return success;

        }

        public async Task<UserRole> GetByIdAsync(int id)
        {

            var key = GetCacheKey(id);
            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var userRole = await _userRolesRepository.SelectByIdAsync(id);
                if (userRole != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    }

                    cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                }
                return userRole;
            });

        }

        public async Task<IEnumerable<UserRole>> GetUserRoles(int userId)
        {
            return await _userRolesRepository.SelectUserRolesByUserId(userId);
        }

        public IQuery QueryAsync()
        {
            throw new NotImplementedException();
        }

        public IQuery<UserRole> QueryAsync2()
        {
            //var query = new EntityReplyQuery(this);
            //return _dbQuery.ConfigureQuery<EntityReply>(query); ;
            throw new NotImplementedException();
        }

        public Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<UserRole>> SelectAsync(params object[] args)
        {
            throw new NotImplementedException();
        }

        private string GetCacheKey(int userId)
        {
            return $"{Key}_{userId.ToString()}";
        }
        
    }
}