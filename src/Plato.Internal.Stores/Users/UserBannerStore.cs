using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Abstractions.Query;
using Plato.Internal.Models.Users;
using Plato.Internal.Repositories.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Plato.Abstractions.Data;


namespace Plato.Internal.Stores.Users
{
    public class UserBannerStore : IUserBannerStore<UserBanner>
    {

        private readonly string _key = CacheKeys.UserBanners.ToString();
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;

        private readonly IUserBannerRepository<UserBanner> _userBannerRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<UserBannerStore> _logger;

        #region "Constrcutor"

        public UserBannerStore(
            IUserBannerRepository<UserBanner> userBannerRepository,
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            ILogger<UserBannerStore> logger
            )
        {
            _userBannerRepository = userBannerRepository;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _logger = logger;

            _cacheEntryOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(12000)
            };

        }

        #endregion

        #region "Implementation"

        public async Task<UserBanner> CreateAsync(UserBanner userBanner)
        {
            if (userBanner == null)
                throw new ArgumentNullException(nameof(userBanner));
            if (userBanner.Id > 0)
                throw new ArgumentOutOfRangeException(nameof(userBanner.Id));
            var newUserPhoto = await _userBannerRepository.InsertUpdateAsync(userBanner);
            if (newUserPhoto != null)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Entry removed from cache of type {0}. Entry key: {1}.",
                        _memoryCache.GetType().Name, _key);
                ClearCache(userBanner);
            }

            return newUserPhoto;
        }

        public Task<bool> DeleteAsync(UserBanner model)
        {
            throw new NotImplementedException();
        }

        public async Task<UserBanner> GetByIdAsync(int id)
        {
            UserBanner userBanner;
            var key = GetCacheKey(LocalCacheKeys.ById, id);
            if (!_memoryCache.TryGetValue(key, out userBanner))
            {
                userBanner = await _userBannerRepository.SelectByIdAsync(id);
                if (userBanner != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    _memoryCache.Set(key, userBanner, _cacheEntryOptions);
                }
            }

            return userBanner;
        }

        public async Task<UserBanner> GetByUserIdAsync(int userId)
        {
            UserBanner userBanner;
            var key = GetCacheKey(LocalCacheKeys.ByUserId, userId);
            if (!_memoryCache.TryGetValue(key, out userBanner))
            {
                userBanner = await _userBannerRepository.SelectByUserIdAsync(userId);
                if (userBanner != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    _memoryCache.Set(key, userBanner, _cacheEntryOptions);
                }
            }

            return userBanner;
        }

        public IQuery QueryAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

        public async Task<UserBanner> UpdateAsync(UserBanner userBanner)
        {
            if (userBanner == null)
                throw new ArgumentNullException(nameof(userBanner));
            if (userBanner.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(userBanner.Id));
            var updatedUserPhoto = await _userBannerRepository.InsertUpdateAsync(userBanner);
            if (updatedUserPhoto != null)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Entry removed from cache of type {0}. Entry key: {1}.",
                        _memoryCache.GetType().Name, _key);
                ClearCache(updatedUserPhoto);
            }
            return updatedUserPhoto;
        }

        #endregion


        #region "Private Methods"


        private string GetCacheKey(LocalCacheKeys cacheKey, object vaule)
        {
            return _key + "_" + cacheKey + "_" + vaule;
        }
        private void ClearCache(UserBanner userBanner)
        {
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ById, userBanner.Id));
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ByUserId, userBanner.UserId));
        }

        private enum LocalCacheKeys
        {
            ById,
            ByUserId
        }

        #endregion


    }
}
