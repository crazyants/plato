using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Abstractions.Data;
using Plato.Abstractions.Query;
using Plato.Models.Users;
using Plato.Repositories.Users;

namespace Plato.Internal.Stores.Users
{
    public class PlatoUserStore : IPlatoUserStore<User>
    {

        #region "Private Variables"

        private readonly string _key = CacheKeys.Users.ToString();
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;

        private readonly IDbQuery _dbQuery;
        private readonly IUserRepository<User> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<PlatoUserStore> _logger;

        #endregion

        #region "constructor"

        public PlatoUserStore(
            IDbQuery dbQuery,
            IUserRepository<User> userRepository,
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            ILogger<PlatoUserStore> logger
        )
        {
            _dbQuery = dbQuery;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _logger = logger;
            _cacheEntryOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(10)
            };
        }

        #endregion

        #region "IPlatoUserStore"

        public async Task<User> CreateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (user.Id > 0)
                throw new ArgumentOutOfRangeException(nameof(user.Id));
            var newUser = await _userRepository.InsertUpdateAsync(user);
            if (newUser != null)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Entry removed from cache of type {0}. Entry key: {1}.",
                        _memoryCache.GetType().Name, _key);
                ClearCache(user);
            }

            return newUser;
        }

        public Task<bool> DeleteAsync(User user)
        {
            throw new NotImplementedException();
            //ClearUserCache(user);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var key = GetCacheKey(LocalCacheKeys.ById, id);
            if (!_memoryCache.TryGetValue(key, out User user))
            {
                user = await _userRepository.SelectByIdAsync(id);
                if (user != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    _memoryCache.Set(key, user, _cacheEntryOptions);
                }
            }

            return user;
        }

        public async Task<User> GetByUserNameNormalizedAsync(string userNameNormalized)
        {
            User user;
            var key = GetCacheKey(LocalCacheKeys.ByUserNameNormalzied, userNameNormalized);
            if (!_memoryCache.TryGetValue(key, out user))
            {
                user = await _userRepository.SelectByUserNameNormalizedAsync(userNameNormalized);
                if (user != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    _memoryCache.Set(key, user, _cacheEntryOptions);
                }
            }

            return user;
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            var key = GetCacheKey(LocalCacheKeys.ByUserName, userName);
            if (!_memoryCache.TryGetValue(key, out User user))
            {
                user = await _userRepository.SelectByUserNameAsync(userName);
                if (user != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, _key);
                    _memoryCache.Set(key, user, _cacheEntryOptions);
                }
            }

            return user;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var key = GetCacheKey(LocalCacheKeys.ByEmail, email);
            if (!_memoryCache.TryGetValue(key, out User user))
            {
                user = await _userRepository.SelectByEmailAsync(email);
                if (user != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, _key);
                    _memoryCache.Set(key, user, _cacheEntryOptions);
                }
            }

            return user;
        }

        public async Task<User> GetByApiKeyAsync(string apiKey)
        {
            var key = GetCacheKey(LocalCacheKeys.ByApiKey, apiKey);
            if (!_memoryCache.TryGetValue(key, out User user))
            {
                user = await _userRepository.SelectByApiKeyAsync(apiKey);
                if (user != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, _key);
                    _memoryCache.Set(key, user, _cacheEntryOptions);
                }
            }

            return user;
        }
        
        public IQuery QueryAsync()
        {
            var query = new UserQuery(this);
            return _dbQuery.ConfigureQuery(query); ;
        }

        public async Task<User> UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (user.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(user.Id));
            var updatedUser = await _userRepository.InsertUpdateAsync(user);
            if (updatedUser != null)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Entry removed from cache of type {0}. Entry key: {1}.",
                        _memoryCache.GetType().Name, _key);
                ClearCache(user);
            }

            return updatedUser;
        }

        public async Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {
            if (!_memoryCache.TryGetValue(_key, out IPagedResults<T> users))
            {
                users = await _userRepository.SelectAsync<T>(args);
                if (users != null)
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, _key);
            }
            return users;
        }

        #endregion

        #region "Private Methods"

        private void ClearCache(User user)
        {
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ById, user.Id));
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ByEmail, user.Email));
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ByUserName, user.UserName));
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ByUserNameNormalzied, user.NormalizedUserName));
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ByApiKey, user.Detail.ApiKey));
        }

        private string GetCacheKey(LocalCacheKeys cacheKey, object vaule)
        {
            return _key + "_" + cacheKey + "_" + vaule;
        }

        private enum LocalCacheKeys
        {
            ById,
            ByEmail,
            ByUserName,
            ByUserNameNormalzied,
            ByApiKey
        }

        #endregion

    }
}