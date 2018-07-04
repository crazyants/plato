using System;
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
    public class UserPhotoStore : IUserPhotoStore<UserPhoto>
    {

        private readonly string _key = CacheKeys.UserPhotos.ToString();
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;
        
        private readonly IUserPhotoRepository<UserPhoto> _userPhotoRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<UserPhotoStore> _logger;

        #region "Constrcutor"

        public UserPhotoStore(
            IUserPhotoRepository<UserPhoto> userPhotoRepository,
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            ILogger<UserPhotoStore> logger)
        {
            _userPhotoRepository = userPhotoRepository;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _logger = logger;

            _cacheEntryOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(10)
            };

        }

        #endregion

        #region "Implementation"

        public async Task<UserPhoto> CreateAsync(UserPhoto userPhoto)
        {
            if (userPhoto == null)
                throw new ArgumentNullException(nameof(userPhoto));
            if (userPhoto.Id > 0)
                throw new ArgumentOutOfRangeException(nameof(userPhoto.Id));
            var newUserPhoto = await _userPhotoRepository.InsertUpdateAsync(userPhoto);
            if (newUserPhoto != null)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Entry removed from cache of type {0}. Entry key: {1}.",
                        _memoryCache.GetType().Name, _key);
                ClearCache(userPhoto);
            }

            return newUserPhoto;
        }

        public Task<bool> DeleteAsync(UserPhoto model)
        {
            throw new NotImplementedException();
        }

        public async Task<UserPhoto> GetByIdAsync(int id)
        {
            UserPhoto userPhoto;
            var key = GetCacheKey(LocalCacheKeys.ById, id);
            if (!_memoryCache.TryGetValue(key, out userPhoto))
            {
                userPhoto = await _userPhotoRepository.SelectByIdAsync(id);
                if (userPhoto != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    _memoryCache.Set(key, userPhoto, _cacheEntryOptions);
                }
            }

            return userPhoto;
        }

        public async Task<UserPhoto> GetByUserIdAsync(int userId)
        {
            var key = GetCacheKey(LocalCacheKeys.ByUserId, userId);
            if (!_memoryCache.TryGetValue(key, out UserPhoto userPhoto))
            {
                userPhoto = await _userPhotoRepository.SelectByUserIdAsync(userId);
                if (userPhoto != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    _memoryCache.Set(key, userPhoto, _cacheEntryOptions);
                }
            }

            return userPhoto;
        }

        public IQuery QueryAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<UserPhoto>> SelectAsync(params object[] args)
        {
            throw new NotImplementedException();
        }

        public async Task<UserPhoto> UpdateAsync(UserPhoto userPhoto)
        {
            if (userPhoto == null)
                throw new ArgumentNullException(nameof(userPhoto));
            if (userPhoto.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(userPhoto.Id));
            var updatedUserPhoto = await _userPhotoRepository.InsertUpdateAsync(userPhoto);
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
        private void ClearCache(UserPhoto userPhoto)
        {
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ById, userPhoto.Id));
            _memoryCache.Remove(GetCacheKey(LocalCacheKeys.ByUserId, userPhoto.UserId));
        }

        private enum LocalCacheKeys
        {
            ById,
            ByUserId
        }

        #endregion
        
    }
}
