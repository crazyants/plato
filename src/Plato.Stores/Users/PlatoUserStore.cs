using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Abstractions.Collections;
using Plato.Abstractions.Query;
using Plato.Models.Users;
using Plato.Repositories.Users;

namespace Plato.Stores.Users
{
    public class PlatoUserStore : IPlatoUserStore
    {

        private enum LocalCacheKeys
        {
            ById,
            ByEmail,
            ByUserName,
            ByUserNameNormalzied,
            ByApiKey,
            
                
        }

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _key = CacheKeys.Users.ToString();
        private readonly ILogger<PlatoUserStore> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly IUserRepository<User> _userRepository;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;

        public PlatoUserStore(
            IUserRepository<User> userRepository,
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            ILogger<PlatoUserStore> logger
        )
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _logger = logger;
            
            _cacheEntryOptions = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = new TimeSpan?(TimeSpan.FromSeconds(10))
            };

        }
        
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
                    _logger.LogDebug("Entry removed from cache of type {0}. Entry key: {1}.", _memoryCache.GetType().Name, _key);
                ClearUserCache(user);
            }
         
            return newUser;

        }

        public Task<bool> DeleteAsync(User user)
        {
            throw new NotImplementedException();
            ClearUserCache(user);
        }

        public async Task<User> GetByIdAsync(int id)
        {

            User user;
            var key = GetCacheKey(LocalCacheKeys.ById, id);
            if (!_memoryCache.TryGetValue(key, out user))
            {
                user = await _userRepository.SelectByIdAsync(id);
                if (user != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.", _memoryCache.GetType().Name, _key);
                    _memoryCache.Set(key, user, _cacheEntryOptions);
                }
            }

            return user;
        }


        private void ClearUserCache(User user)
        {
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ById, user.Id));
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ByEmail, user.Email));
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ByUserName, user.UserName));
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ByUserNameNormalzied, user.NormalizedUserName));
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ByApiKey, user.Detail.ApiKey));
        }

        private string GetCacheKey(LocalCacheKeys cacheKey, object vaule)
        {
            // 376.5875 ms,    String interpolation
            // 293.1515 ms,    Concat(+)
            // 369.2315 ms,    String Format
            return _key + "_" + cacheKey + "_" + vaule;

        }

        public async Task<User> GetByUserNameNormalizedAsync(string userNameNormalized)
        {
            User user;
            var key = GetCacheKey(LocalCacheKeys.ByUserNameNormalzied, userNameNormalized);
            if (!_memoryCache.TryGetValue(key + userNameNormalized, out user))
            {
                user = await _userRepository.SelectByUserNameNormalizedAsync(userNameNormalized);
                if (user != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.", _memoryCache.GetType().Name, _key);
                    _memoryCache.Set(key, user, _cacheEntryOptions);
                }
            }

            return user;
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            User user;
            var key = GetCacheKey(LocalCacheKeys.ByUserName, userName);
            if (!_memoryCache.TryGetValue(key + userName, out user))
            {
                user = await _userRepository.SelectByUserNameAsync(userName);
                if (user != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.", _memoryCache.GetType().Name, _key);
                    _memoryCache.Set(key, user, _cacheEntryOptions);
                }
            }

            return user;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            User user;
            var key = GetCacheKey(LocalCacheKeys.ByEmail, email);
            if (!_memoryCache.TryGetValue(key + email, out user))
            {
                user = await _userRepository.SelectByEmailAsync(email);
                if (user != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.", _memoryCache.GetType().Name, _key);
                    _memoryCache.Set(key, user, _cacheEntryOptions);
                }
            }

            return user;
        }

        public async Task<User> GetByApiKeyAsync(string apiKey)
        {
            User user;
            var key = GetCacheKey(LocalCacheKeys.ByApiKey, apiKey);
            if (!_memoryCache.TryGetValue(key, out user))
            {
                user = await _userRepository.SelectByApiKeyAsync(apiKey);
                if (user != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.", _memoryCache.GetType().Name, _key);
                    _memoryCache.Set(key, user, _cacheEntryOptions);
                }
            }

            return user;
        }


        public IQuery QueryAsync()
        {
            return new UserQuery(this);
        }

        public async Task<User> UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (user.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(user.Id));
            var updatedUser  = await _userRepository.InsertUpdateAsync(user);
            if (updatedUser != null)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Entry removed from cache of type {0}. Entry key: {1}.", _memoryCache.GetType().Name, _key);
                ClearUserCache(user);
            }

            return updatedUser;
        }

        public async Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {
            IPagedResults<T> users;
            if (!_memoryCache.TryGetValue(_key, out users))
            {
                users = await _userRepository.SelectAsync<T>(args);
                if (users != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.", _memoryCache.GetType().Name, _key);
                    //_memoryCache.Set(_key, users);
                }
            }
            return users;
        }

    }

}