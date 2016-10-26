using System;
using System.Collections.Generic;
using Plato.Data;
using System.Data;
using Plato.Models.Users;
using Plato.Abstractions.Extensions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Plato.Repositories.Users
{

    public class UserRepository : IUserRepository<User>
    {

        #region "Private Variables"

        private IDbContext _dbContext;
        private IUserSecretRepository<UserSecret> _userSecretRepository;
        private IUserDetailRepository<UserDetail> _userDetailRepository;
        private IUserPhotoRepository<UserPhoto> _userPhotoRepository;
        private ILogger<UserSecretRepository> _logger;

        #endregion

        #region "Constructor"

        public UserRepository(
            IDbContext dbContext,
            IUserSecretRepository<UserSecret> userSecretRepository,
            IUserDetailRepository<UserDetail> userDetailRepository,
            IUserPhotoRepository<UserPhoto> userPhotoRepository,
            ILogger<UserSecretRepository> logger)
        {
            _dbContext = dbContext;
            _userSecretRepository = userSecretRepository;
            _userDetailRepository = userDetailRepository;
            _userPhotoRepository = userPhotoRepository;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<User> InsertUpdateAsync(User user)
        {

            if (_userSecretRepository == null)
                throw new ArgumentNullException(nameof(_userSecretRepository));

            if (_userDetailRepository == null)
                throw new ArgumentNullException(nameof(_userDetailRepository));

            var id = await InsertUpdateInternal(
                user.Id,         
                user.UserName,
                user.NormalizedUserName,
                user.Email,
                user.NormalizedEmail,
                user.DisplayName,
                user.SamAccountName);

            if (id > 0)
            {

                // secerts

                if (user.Secret == null)
                    user.Secret = new UserSecret();
                if (user.Id == 0 || user.Secret.UserId == 0)
                    user.Secret.UserId = id;
                await _userSecretRepository.InsertUpdateAsync(user.Secret);

                // detail

                if (user.Detail == null)
                    user.Detail = new UserDetail();
                if (user.Id == 0 || user.Detail.UserId == 0)
                    user.Detail.UserId = id;
                await _userDetailRepository.InsertUpdateAsync(user.Detail);
                
                // photos

                if (user.Photo == null)
                    user.Photo = new UserPhoto();
                if (user.Id == 0 || user.Photo.UserId == 0)
                    user.Photo.UserId = id;
                await _userPhotoRepository.InsertUpdateAsync(user.Photo);
             
                // return

                return await SelectByIdAsync(id);

            }

            return null;    

        }
        
        public async Task<User> SelectByIdAsync(int id)
        {
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectUser", id);

                return await BuildUserFromResultSets(reader);
            }
        }

        public Task<IEnumerable<User>> SelectPagedAsync(int pageIndex, int pageSize, object options)
        {

            throw new NotImplementedException();
            
        }

        public async Task<User> SelectByUserNameNormalizedAsync(string userNameNormalized)
        {

            if (string.IsNullOrEmpty(userNameNormalized))
                throw new ArgumentNullException(nameof(userNameNormalized));

            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectUserByUserNameNormalized", userNameNormalized);

                return await BuildUserFromResultSets(reader);
            }
        }
        
        public async Task<User> SelectByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));

            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectUserByEmail", email);

                return await BuildUserFromResultSets(reader);
            }
        }
        
        public async Task<User> SelectByUserNameAsync(string userName)
        {

            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectUserByUserName", userName);

                return await BuildUserFromResultSets(reader);
            }

        }

        public async Task<User> SelectByUserNameAndPasswordAsync(string userName, string password)
        {

            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
            
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectUserByUserNameAndPassword",
                  userName,
                  password);

                return await BuildUserFromResultSets(reader);
            }

        }

        public async Task<User> SelectByEmailAndPasswordAsync(string email, string password)
        {

            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectUserByEmailAndPassword",
                  email,
                  password);

                return await BuildUserFromResultSets(reader);
            }

        }

        public async Task<User> SelectByApiKeyAsync(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey));
       
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectUserByApiKey", apiKey);

                return await BuildUserFromResultSets(reader);
            }

        }

        #endregion

        #region "Private Methods"
        
        private async Task<User> BuildUserFromResultSets(DbDataReader reader)
        {
            var user = new User();
            if (reader != null)
            {
                await reader.ReadAsync();
                user.PopulateModel(reader);

                if (await reader.NextResultAsync())
                {
                    await reader.ReadAsync();
                    user.Secret = new UserSecret();
                    user.Secret.PopulateModel(reader);
                }

                if (await reader.NextResultAsync())
                {
                    await reader.ReadAsync();
                    user.Detail = new UserDetail();
                    user.Detail.PopulateModel(reader);
                }

                if (await reader.NextResultAsync())
                {
                    await reader.ReadAsync();
                    user.Photo = new UserPhoto();
                    user.Photo.PopulateModel(reader);
                }

            }

            return user;

        }
        
        private async Task<int> InsertUpdateInternal(
            int id,      
            string userName,
            string normalizedUserName,
            string email,
            string normalizedEmail,
            string displayName,
            string samAccountName)
        {

            var userId = 0;
            using (var context = _dbContext)
            {

                userId = await context.ExecuteScalarAsync<int>(
                  CommandType.StoredProcedure,
                  "plato_sp_InsertUpdateUser",
                    id,           
                    userName.ToEmptyIfNull(),
                    normalizedUserName.ToEmptyIfNull(),
                    email.ToEmptyIfNull(),
                    normalizedEmail.ToEmptyIfNull(),
                    displayName.ToEmptyIfNull(),
                    samAccountName.ToEmptyIfNull());

            }
                       
            return userId;

        }

        #endregion

    }
}
