using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Abstractions.Collections;
using Plato.Abstractions.Extensions;
using Plato.Data;
using Plato.Models.Roles;
using Plato.Models.Users;

namespace Plato.Repositories.Users
{
    public class UserRepository : IUserRepository<User>
    {
        #region "Constructor"

        public UserRepository(
            IDbContext dbContext,
            IUserSecretRepository<UserSecret> userSecretRepository,
            IUserDetailRepository<UserDetail> userDetailRepository,
            IUserPhotoRepository<UserPhoto> userPhotoRepository,
            IUserRolesRepository<UserRole> userRolesRepository,
            ILogger<UserSecretRepository> logger)
        {
            _dbContext = dbContext;
            _userSecretRepository = userSecretRepository;
            _userDetailRepository = userDetailRepository;
            _userPhotoRepository = userPhotoRepository;
            _userRolesRepository = userRolesRepository;
            _logger = logger;
        }

        #endregion

        #region "Private Variables"

        private readonly IDbContext _dbContext;
        private readonly IUserSecretRepository<UserSecret> _userSecretRepository;
        private readonly IUserDetailRepository<UserDetail> _userDetailRepository;
        private readonly IUserPhotoRepository<UserPhoto> _userPhotoRepository;
        private readonly IUserRolesRepository<UserRole> _userRolesRepository;
        private readonly ILogger<UserSecretRepository> _logger;

        #endregion

        #region "Implementation"

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<User> InsertUpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // ensure we have our dependencies
            if (_userSecretRepository == null)
                throw new ArgumentNullException(nameof(_userSecretRepository));

            if (_userDetailRepository == null)
                throw new ArgumentNullException(nameof(_userDetailRepository));

            if (_userRolesRepository == null)
                throw new ArgumentNullException(nameof(_userRolesRepository));

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
                if ((user.Id == 0) || (user.Secret.UserId == 0))
                    user.Secret.UserId = id;
                await _userSecretRepository.InsertUpdateAsync(user.Secret);

                // detail

                if (user.Detail == null)
                    user.Detail = new UserDetail();
                if ((user.Id == 0) || (user.Detail.UserId == 0))
                    user.Detail.UserId = id;
                await _userDetailRepository.InsertUpdateAsync(user.Detail);

                // photo

                if (user.Photo == null)
                    user.Photo = new UserPhoto();
                if ((user.Id == 0) || (user.Photo.UserId == 0))
                    user.Photo.UserId = id;
                await _userPhotoRepository.InsertUpdateAsync(user.Photo);

                // roles

                if (user.RoleNames.Count > 0)
                {
                    //_userRolesRepository.InsertUserRole()
                }
                //if (user.UserRoles == null)
                //    user.UserRoles = new List<Role>();
                //if (user.Id == 0 || user.Photo.UserId == 0)
                //    user.Photo.UserId = id;
                //await _userPhotoRepository.InsertUpdateAsync(user.Photo);

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

        public async Task<IPagedResults<T>> SelectAsync<T>(params object[] inputParameters) where T : class
        {
            PagedResults<T> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectUsersPaged",
                    inputParameters
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<T>();
                    while (await reader.ReadAsync())
                    {
                        var user = new User();
                        user.PopulateModel(reader);
                        output.Add((T) Convert.ChangeType(user, typeof(T)));
                    }

                    if (await reader.NextResultAsync())
                    {
                        await reader.ReadAsync();
                        output.PopulateTotal(reader);
                    }
                }
            }

            return output;
        }

        #endregion

        #region "Private Methods"

        private async Task<User> BuildUserFromResultSets(DbDataReader reader)
        {
            var user = new User();
            if ((reader != null) && (reader.HasRows))
            {
                
                await reader.ReadAsync();
                user.PopulateModel(reader);

                // user

                if (await reader.NextResultAsync())
                {
                    await reader.ReadAsync();
                    user.Secret = new UserSecret(reader);
                }

                // detail

                if (await reader.NextResultAsync())
                {
                    await reader.ReadAsync();
                    user.Detail = new UserDetail(reader);
                }

                // photo

                if (await reader.NextResultAsync())
                {
                    await reader.ReadAsync();
                    user.Photo = new UserPhoto(reader);
                }

                // roles

                if (await reader.NextResultAsync())
                    while (await reader.ReadAsync())
                        user.UserRoles.Add(new Role(reader));
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
            using (var context = _dbContext)
            {
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "plato_sp_InsertUpdateUser",
                    id,
                    userName.ToEmptyIfNull().TrimToSize(255),
                    normalizedUserName.ToEmptyIfNull().TrimToSize(255),
                    email.ToEmptyIfNull().TrimToSize(255),
                    normalizedEmail.ToEmptyIfNull().TrimToSize(255),
                    displayName.ToEmptyIfNull().TrimToSize(255),
                    samAccountName.ToEmptyIfNull().TrimToSize(255));
            }
        }

        #endregion
    }
}