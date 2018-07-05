using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Users;
using Plato.Internal.Repositories.Users;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Internal.Stores.Users
{
    public class UserPhotoStore : IUserPhotoStore<UserPhoto>
    {

        private readonly ICacheManager _cacheManager;
        private readonly IUserPhotoRepository<UserPhoto> _userPhotoRepository;
        private readonly ILogger<UserPhotoStore> _logger;

        #region "Constrcutor"

        public UserPhotoStore(
            IUserPhotoRepository<UserPhoto> userPhotoRepository,
            ILogger<UserPhotoStore> logger,
            ICacheManager cacheManager)
        {
            _userPhotoRepository = userPhotoRepository;
            _cacheManager = cacheManager;
            _logger = logger;
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
                _cacheManager.CancelTokens(this.GetType(), "ById", userPhoto.Id);
                _cacheManager.CancelTokens(this.GetType(), userPhoto.UserId);
            }

            return newUserPhoto;
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
                _cacheManager.CancelTokens(this.GetType(), "ById", userPhoto.Id);
                _cacheManager.CancelTokens(this.GetType(), userPhoto.UserId);
            }
            return updatedUserPhoto;
        }
        
        public Task<bool> DeleteAsync(UserPhoto model)
        {
            throw new NotImplementedException();
        }

        public async Task<UserPhoto> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), "ById", id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _userPhotoRepository.SelectByIdAsync(id));

        }

        public async Task<UserPhoto> GetByUserIdAsync(int userId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), userId);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _userPhotoRepository.SelectByUserIdAsync(userId));
        }

        public IQuery<UserPhoto> QueryAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<UserPhoto>> SelectAsync(params object[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
        
    }

}
