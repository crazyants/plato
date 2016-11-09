using System;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _key = CacheKeys.Users.ToString();
        private readonly ILogger<PlatoUserStore> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly IUserRepository<User> _userRepository;

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
                _memoryCache.Remove(_key);
            }
         
            return newUser;

        }

        public Task<bool> DeleteAsync(User model)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetByIdAsync(int id)
        {

            User user;
            if (!_memoryCache.TryGetValue(_key, out user))
            {
                user = await _userRepository.SelectByIdAsync(id);
                if (user != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.", _memoryCache.GetType().Name, _key);
                    _memoryCache.Set(_key, user);
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
                _memoryCache.Remove(_key);
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