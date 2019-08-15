using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Users;
using Plato.Internal.Repositories.Users;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Internal.Stores.Users
{
    public class UserBannerStore : IUserBannerStore<UserBanner>
    {

        private readonly string _key = "UserBanner";

        private readonly MemoryCacheEntryOptions _cacheEntryOptions;
        private readonly IUserBannerRepository<UserBanner> _userBannerRepository;
        private readonly ILogger<UserBannerStore> _logger;
        private readonly IMemoryCache _memoryCache;

        public UserBannerStore(
            IUserBannerRepository<UserBanner> userBannerRepository,
            ILogger<UserBannerStore> logger,
            IMemoryCache memoryCache)
        {
            _userBannerRepository = userBannerRepository;
            _memoryCache = memoryCache;
            _logger = logger;

            _cacheEntryOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(12000)
            };

        }

        public async Task<UserBanner> CreateAsync(UserBanner userBanner)
        {
            if (userBanner == null)
                throw new ArgumentNullException(nameof(userBanner));
            if (userBanner.Id > 0)
                throw new ArgumentOutOfRangeException(nameof(userBanner.Id));
            var result = await _userBannerRepository.InsertUpdateAsync(userBanner);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
        }

        public Task<bool> DeleteAsync(UserBanner model)
        {
            throw new NotImplementedException();
        }

        public async Task<UserBanner> GetByIdAsync(int id)
        {
            var key = GetCacheKey(LocalCacheKeys.ById, id);
            if (!_memoryCache.TryGetValue(key, out UserBanner userBanner))
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
            var key = GetCacheKey(LocalCacheKeys.ByUserId, userId);
            if (!_memoryCache.TryGetValue(key, out UserBanner userBanner))
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

        public IQuery<UserBanner> QueryAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<UserBanner>> SelectAsync(IDbDataParameter[] dbParams)
        {
            throw new NotImplementedException();
        }

        public async Task<UserBanner> UpdateAsync(UserBanner userBanner)
        {
            if (userBanner == null)
                throw new ArgumentNullException(nameof(userBanner));
            if (userBanner.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(userBanner.Id));
            var result = await _userBannerRepository.InsertUpdateAsync(userBanner);
            if (result != null)
            {
                CancelTokens(result);
            }
            return result;
        }


        public void CancelTokens(UserBanner model)
        {
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ById, model.Id));
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ByUserId, model.UserId));
        }
        
        private string GetCacheKey(LocalCacheKeys cacheKey, object value)
        {
            return _key + "_" + cacheKey + "_" + value;
        }

        private enum LocalCacheKeys
        {
            ById,
            ByUserId
        }
        
    }

}
