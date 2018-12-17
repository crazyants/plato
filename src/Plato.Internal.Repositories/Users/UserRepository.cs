using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Users;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Roles;

namespace Plato.Internal.Repositories.Users
{
    public class UserRepository : IUserRepository<User>
    {

        #region "Private Variables"

        private readonly IUserDataRepository<UserData> _userDataRepository;
        private readonly ILogger<UserSecretRepository> _logger;
        private readonly IDbContext _dbContext;

        #endregion

        #region "Constructor"

        public UserRepository(
            IUserSecretRepository<UserSecret> userSecretRepository,
            IUserDataRepository<UserData> userDataRepository,
            ILogger<UserSecretRepository> logger,
            IDbContext dbContext)
        {
            _userDataRepository = userDataRepository;
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion
        
        #region "Implementation"

        public Task<bool> DeleteAsync(int id)
        {
            // TODO
            throw new NotImplementedException();
        }

        public async Task<User> InsertUpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var userId = await InsertUpdateInternal(
                user.Id,
                user.PrimaryRoleId,
                user.UserName,
                user.NormalizedUserName,
                user.Email,
                user.NormalizedEmail,
                user.EmailConfirmed,
                user.DisplayName,
                user.FirstName,
                user.LastName,
                user.Alias,
                user.SamAccountName,
                user.PasswordHash,
                user.PasswordExpiryDate,
                user.PasswordUpdatedDate,
                user.SecurityStamp,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                user.LockoutEnd,
                user.LockoutEnabled,
                user.AccessFailedCount,
                user.ResetToken,
                user.ConfirmationToken,
                user.ApiKey,
                user.TimeZone,
                user.ObserveDst,
                user.Culture,
                user.Theme,
                user.IpV4Address,
                user.IpV6Address,
                user.Visits,
                user.VisitsUpdatedDate,
                user.MinutesActive,
                user.MinutesActiveUpdatedDate,
                user.Reputation,
                user.ReputationUpdatedDate,
                user.Rank,
                user.RankUpdatedDate,
                user.Signature,
                user.IsSpam,
                user.IsBanned,
                user.BanExpiryDate,
                user.CreatedUserId,
                user.CreatedDate,
                user.ModifiedUserId,
                user.ModifiedDate,
                user.LastLoginDate,
                user.Data);

            if (userId > 0)
            {
                return await SelectByIdAsync(userId);
            }

            return null;
        }

        public async Task<User> SelectByIdAsync(int id)
        {

            User user = null;
            using (var context = _dbContext)
            {
                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation(
                            $"SelectUser for Id {id} failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserById", id);

                user = await BuildUserFromResultSets(reader);
            }

            return user;

        }

        public async Task<User> SelectByUserNameNormalizedAsync(string userNameNormalized)
        {

            if (string.IsNullOrEmpty(userNameNormalized))
                throw new ArgumentNullException(nameof(userNameNormalized));

            User user = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserByUserNameNormalized",
                    userNameNormalized.TrimToSize(255));

                user = await BuildUserFromResultSets(reader);
            }

            return user;

        }

        public async Task<User> SelectByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }
                
            User user = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserByEmail", 
                    email.TrimToSize(255));

                user = await BuildUserFromResultSets(reader);
            }

            return user;

        }

        public async Task<User> SelectByEmailNormalizedAsync(string emailNormalized)
        {
            if (string.IsNullOrEmpty(emailNormalized))
            {
                throw new ArgumentNullException(nameof(emailNormalized));
            }

            User user = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserByEmailNormalized",
                    emailNormalized.TrimToSize(255));

                user = await BuildUserFromResultSets(reader);
            }

            return user;
        }

        public async Task<User> SelectByUserNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }
                
            User user = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserByUserName", 
                    userName.TrimToSize(255));
                user = await BuildUserFromResultSets(reader);
            }

            return user;

        }

        public async Task<User> SelectByUserNameAndPasswordAsync(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            User user = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectUserByUserNameAndPassword failed with the following error {args.Exception.Message}");
                };
                
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserByUserNameAndPassword",
                    userName.TrimToSize(255),
                    password.TrimToSize(255));

                user = await BuildUserFromResultSets(reader);
            }

            return user;

        }

        public async Task<User> SelectByEmailAndPasswordAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            User user = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectUserByEmailAndPassword failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserByEmailAndPassword",
                    email.TrimToSize(255),
                    password.TrimToSize(255));

                user = await BuildUserFromResultSets(reader);
            }

            return user;
        }

        public async Task<User> SelectByResetTokenAsync(string resetToken)
        {
            if (string.IsNullOrEmpty(resetToken))
            {
                throw new ArgumentNullException(nameof(resetToken));
            }

            User user = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserByResetToken",
                    resetToken.TrimToSize(255));
                user = await BuildUserFromResultSets(reader);
            }

            return user;
        }

        public async Task<User> SelectByConfirmationTokenAsync(string confirmationToken)
        {
            if (string.IsNullOrEmpty(confirmationToken))
            {
                throw new ArgumentNullException(nameof(confirmationToken));
            }

            User user = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserByConfirmationToken",
                    confirmationToken.TrimToSize(255));
                user = await BuildUserFromResultSets(reader);
            }

            return user;
        }

        public async Task<User> SelectByApiKeyAsync(string apiKey)
        {
            if (String.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }
                
            User user = null;
            using (var context = _dbContext)
            {

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserByApiKey",
                    apiKey.TrimToSize(255));

                user = await BuildUserFromResultSets(reader);
            }

            return user;

        }
        
        public async Task<IPagedResults<User>> SelectAsync(params object[] inputParams)
        {
            PagedResults<User> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUsersPaged",
                    inputParams);
                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<User>();
                    while (await reader.ReadAsync())
                    {
                        var user = new User();
                        user.PopulateModel(reader);
                        output.Data.Add(user);
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
            User user = null;
            if ((reader != null) && (reader.HasRows))
            {
                // user

                user = new User();
                await reader.ReadAsync();
                if (reader.HasRows)
                {
                    user.PopulateModel(reader);
                }

                // data

                if (await reader.NextResultAsync())
                {
                    if (reader.HasRows)
                    {
                        var data = new List<UserData>();
                        while (await reader.ReadAsync())
                        {
                            data.Add(new UserData(reader));
                        }
                        user.Data = data;
                    }
                }

                // roles

                if (await reader.NextResultAsync())
                {
                    if (reader.HasRows)
                    {
                        var data = new List<Role>();
                        while (await reader.ReadAsync())
                        {
                            data.Add(new Role(reader));
                        }
                        user.UserRoles = data;
                    }
                }

            }

            return user;

        }

        private async Task<int> InsertUpdateInternal(
            int id,
            int primaryRoleId,
            string userName,
            string normalizedUserName,
            string email,
            string normalizedEmail,
            bool emailConfirmed,
            string displayName,
            string firstName,
            string lastName,
            string alias,
            string samAccountName,
            string passwordHash,
            DateTimeOffset? passwordExpiryDate,
            DateTimeOffset? passwordUpdatedDate,
            string securityStamp,
            string phoneNumber,
            bool phoneNumberConfirmed,
            bool twoFactorEnabled,
            DateTimeOffset? lockoutEnd,
            bool lockoutEnabled,
            int accessFailedCount,
            string resetToken,
            string confirmationToken,
            string apiKey,
            string timeZone,
            bool observeDst,
            string culture,
            string theme,
            string ipv4Address,
            string ipv6Address,
            int visits,
            DateTimeOffset? visitsUpdatedDate,
            int minutesActive,
            DateTimeOffset? minutesActiveUpdatedDate,
            int reputation,
            DateTimeOffset? reputationUpdatedDate,
            int rank,
            DateTimeOffset? rankUpdatedDate,
            string signature,
            bool isSpam,
            bool isBanned,
            DateTimeOffset? banExpiryDate,
            int createdUserId,
            DateTimeOffset? createdDate,
            int modifiedUserId,
            DateTimeOffset? modifiedDate,
            DateTimeOffset? lastLoginDate,
            IEnumerable<UserData> data)
        {
     
            var userId = 0;
            using (var context = _dbContext)
            {

                userId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUser",
                    id,
                    primaryRoleId,
                    userName.ToEmptyIfNull().TrimToSize(255),
                    normalizedUserName.ToEmptyIfNull().TrimToSize(255),
                    email.ToEmptyIfNull().TrimToSize(255),
                    normalizedEmail.ToEmptyIfNull().TrimToSize(255),
                    emailConfirmed,
                    displayName.ToEmptyIfNull().TrimToSize(255),
                    firstName.ToEmptyIfNull().TrimToSize(255),
                    lastName.ToEmptyIfNull().TrimToSize(255),
                    alias.ToEmptyIfNull().TrimToSize(255),
                    samAccountName.ToEmptyIfNull().TrimToSize(255),
                    passwordHash.ToEmptyIfNull().TrimToSize(255),
                    passwordExpiryDate,
                    passwordUpdatedDate,
                    securityStamp.ToEmptyIfNull().TrimToSize(255),
                    phoneNumber.ToEmptyIfNull().TrimToSize(255),
                    phoneNumberConfirmed,
                    twoFactorEnabled,
                    lockoutEnd,
                    lockoutEnabled,
                    accessFailedCount,
                    resetToken.ToEmptyIfNull().TrimToSize(255),
                    confirmationToken.ToEmptyIfNull().TrimToSize(255),
                    apiKey.ToEmptyIfNull().TrimToSize(255),
                    timeZone.ToEmptyIfNull().TrimToSize(255),
                    observeDst,
                    culture.ToEmptyIfNull().TrimToSize(50),
                    theme.ToEmptyIfNull().TrimToSize(50),
                    ipv4Address.ToEmptyIfNull().TrimToSize(20),
                    ipv6Address.ToEmptyIfNull().TrimToSize(50),
                    visits,
                    visitsUpdatedDate.ToDateIfNull(),
                    minutesActive,
                    minutesActiveUpdatedDate.ToDateIfNull(),
                    reputation,
                    reputationUpdatedDate.ToDateIfNull(),
                    rank,
                    rankUpdatedDate.ToDateIfNull(),
                    signature.ToEmptyIfNull(),
                    isSpam,
                    isBanned,
                    banExpiryDate,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    modifiedUserId,
                    modifiedDate.ToDateIfNull(),
                    lastLoginDate.ToDateIfNull(),
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output)
                );
            }

            // Add user data
            if (userId > 0)
            {
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        item.UserId = userId;
                        await _userDataRepository.InsertUpdateAsync(item);
                    }
                }

            }
            
            return userId;

        }

        #endregion
    }
}