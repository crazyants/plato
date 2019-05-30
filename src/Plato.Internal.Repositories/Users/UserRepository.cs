using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Users;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions.Extensions;
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

        public async Task<User> SelectByIdAsync(int id)
        {

            User user = null;
            using (var context = _dbContext)
            {
                user = await context.ExecuteReaderAsync<User>(
                    CommandType.StoredProcedure,
                    "SelectUserById",
                    async reader => await BuildUserFromResultSets(reader),
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return user;

        }

        public async Task<User> SelectByUserNameNormalizedAsync(string userNameNormalized)
        {

            if (string.IsNullOrEmpty(userNameNormalized))
            {
                throw new ArgumentNullException(nameof(userNameNormalized));
            }
                
            User user = null;
            using (var context = _dbContext)
            {
                user = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserByUserNameNormalized",
                    async reader => await BuildUserFromResultSets(reader),
                    new IDbDataParameter[]
                    {
                        new DbParam("NormalizedUserName", DbType.String, 255, userNameNormalized), 
                    });
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
                user = await context.ExecuteReaderAsync<User>(
                    CommandType.StoredProcedure,
                    "SelectUserByEmail",
                    async reader => await BuildUserFromResultSets(reader),
                    new IDbDataParameter[]
                    {
                        new DbParam("Email", DbType.String, 255, email)
                    });
            }

            return user;

        }

        public async Task<User> SelectByEmailNormalizedAsync(string normalizedEmail)
        {
            if (string.IsNullOrEmpty(normalizedEmail))
            {
                throw new ArgumentNullException(nameof(normalizedEmail));
            }

            User user = null;
            using (var context = _dbContext)
            {
                user = await context.ExecuteReaderAsync<User>(
                    CommandType.StoredProcedure,
                    "SelectUserByEmailNormalized",
                    async reader => await BuildUserFromResultSets(reader),
                    new IDbDataParameter[]
                    {
                        new DbParam("NormalizedEmail", DbType.String, 255, normalizedEmail),
                    });
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
                user = await context.ExecuteReaderAsync<User>(
                    CommandType.StoredProcedure,
                    "SelectUserByUserName",
                    async reader => await BuildUserFromResultSets(reader),
                    new IDbDataParameter[]
                    {
                        new DbParam("UserName", DbType.String, 255, userName)
                    });
            }

            return user;

        }

        public async Task<User> SelectByUserNameAndPasswordAsync(string userName, string password)
        {

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }
                
            User user = null;
            using (var context = _dbContext)
            {
                user = await context.ExecuteReaderAsync<User>(
                    CommandType.StoredProcedure,
                    "SelectUserByUserNameAndPassword",
                    async reader => await BuildUserFromResultSets(reader),
                    new IDbDataParameter[]
                    {
                        new DbParam("UserName", DbType.String, 255, userName),
                        new DbParam("Password", DbType.String, 255, password),
                    });
            }

            return user;

        }

        public async Task<User> SelectByEmailAndPasswordAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            User user = null;
            using (var context = _dbContext)
            {
                user = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserByEmailAndPassword",
                    async reader => await BuildUserFromResultSets(reader),
                    new IDbDataParameter[]
                    {
                        new DbParam("Email", DbType.String, 255, email),
                        new DbParam("Password", DbType.String, 255, password),
                    });
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
                user = await context.ExecuteReaderAsync<User>(
                    CommandType.StoredProcedure,
                    "SelectUserByResetToken",
                    async reader => await BuildUserFromResultSets(reader),
                    new IDbDataParameter[]
                    {
                        new DbParam("ResetToken", DbType.String, 255, resetToken)
                    });
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
                user = await context.ExecuteReaderAsync<User>(
                    CommandType.StoredProcedure,
                    "SelectUserByConfirmationToken",
                    async reader => await BuildUserFromResultSets(reader),
                    new IDbDataParameter[]
                    {
                        new DbParam("ConfirmationToken", DbType.String, 255, confirmationToken),
                    });
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
                user = await context.ExecuteReaderAsync<User>(
                    CommandType.StoredProcedure,
                    "SelectUserByApiKey",
                    async reader => await BuildUserFromResultSets(reader),
                    new IDbDataParameter[]
                    {
                        new DbParam("ApiKey", DbType.String, 255, apiKey)
                    });
            }

            return user;

        }
        
        public async Task<IPagedResults<User>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<User> results = null;
            using (var context = _dbContext)
            {
                results = await context.ExecuteReaderAsync<IPagedResults<User>>(
                    CommandType.StoredProcedure,
                    "SelectUsersPaged",
                    async reader =>
                    {

                        if ((reader != null) && (reader.HasRows))
                        {
                            var output = new PagedResults<User>();

                            while (await reader.ReadAsync())
                            {
                                var user = new User();
                                user.PopulateModel(reader);
                                output.Data.Add(user);
                            }

                            if (await reader.NextResultAsync())
                            {
                                if (reader.HasRows)
                                {
                                    await reader.ReadAsync();
                                    output.PopulateTotal(reader);
                                }
                            }

                            return output;
                        }

                        return null;
                    },
                    dbParams);
              
            }

            return results;

        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting user with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteUserById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

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
                user.PhotoUrl,
                user.PhotoColor,
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
                user.Biography,
                user.Location,
                user.Url,
                user.Visits,
                user.VisitsUpdatedDate,
                user.MinutesActive,
                user.MinutesActiveUpdatedDate,
                user.Reputation,
                user.ReputationUpdatedDate,
                user.Rank,
                user.RankUpdatedDate,
                user.Signature,
                user.SignatureHtml,
                user.IsSpam,
                user.IsSpamUpdatedUserId,
                user.IsSpamUpdatedDate,
                user.IsVerified,
                user.IsVerifiedUpdatedUserId,
                user.IsVerifiedUpdatedDate,
                user.IsBanned,
                user.IsBannedUpdatedUserId,
                user.IsBannedUpdatedDate,
                user.IsBannedExpiryDate,
                (short)user.UserType,
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
                            var role = new Role();
                            role.PopulateModel(reader);
                            data.Add(role);
                        }
                        user.UserRoles = data;
                    }
                }

                if (user.UserRoles != null)
                {
                    user.RoleNames = user.UserRoles.Select(r => r.Name).ToList();
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
            string photoUrl,
            string photoColor,
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
            string bio,
            string location,
            string url,
            int visits,
            DateTimeOffset? visitsUpdatedDate,
            int minutesActive,
            DateTimeOffset? minutesActiveUpdatedDate,
            int reputation,
            DateTimeOffset? reputationUpdatedDate,
            int rank,
            DateTimeOffset? rankUpdatedDate,
            string signature,
            string signatureHtml,
            bool isSpam,
            int isSpamUpdatedUserId,
            DateTimeOffset? isSpamUpdatedDate,
            bool isVerified,
            int isVerifiedUpdatedUserId,
            DateTimeOffset? isVerifiedUpdatedDate,
            bool isBanned,
            int isBannedUpdatedUserId,
            DateTimeOffset? isBannedUpdatedDate,
            DateTimeOffset? isBannedExpiryDate,
            short userType,
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
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("PrimaryRoleId", DbType.Int32, primaryRoleId),
                        new DbParam("UserName", DbType.String, 255, userName),
                        new DbParam("NormalizedUserName", DbType.String, 255, normalizedUserName.ToEmptyIfNull()),
                        new DbParam("Email", DbType.String, 255, email.ToEmptyIfNull()),
                        new DbParam("NormalizedEmail", DbType.String, 255, normalizedEmail.ToEmptyIfNull()),
                        new DbParam("EmailConfirmed", DbType.Boolean, emailConfirmed),
                        new DbParam("DisplayName", DbType.String, 255, displayName.ToEmptyIfNull()),
                        new DbParam("FirstName", DbType.String, 255, firstName.ToEmptyIfNull()),
                        new DbParam("LastName", DbType.String, 255, lastName.ToEmptyIfNull()),
                        new DbParam("Alias", DbType.String, 255, alias.ToEmptyIfNull()),
                        new DbParam("PhotoUrl", DbType.String, 255, photoUrl.ToEmptyIfNull()),
                        new DbParam("PhotoColor", DbType.String, 6, photoColor.ToEmptyIfNull()),
                        new DbParam("SamAccountName", DbType.String, 255, samAccountName.ToEmptyIfNull()),
                        new DbParam("PasswordHash", DbType.String, 255, passwordHash.ToEmptyIfNull()),
                        new DbParam("PasswordExpiryDate", DbType.DateTimeOffset, passwordExpiryDate),
                        new DbParam("PasswordUpdatedDate", DbType.DateTimeOffset, passwordUpdatedDate),
                        new DbParam("SecurityStamp", DbType.String, 255, securityStamp.ToEmptyIfNull()),
                        new DbParam("PhoneNumber", DbType.String, 255, phoneNumber.ToEmptyIfNull()),
                        new DbParam("PhoneNumberConfirmed", DbType.Boolean, phoneNumberConfirmed),
                        new DbParam("TwoFactorEnabled", DbType.Boolean, twoFactorEnabled),
                        new DbParam("LockoutEnd", DbType.DateTimeOffset, lockoutEnd),
                        new DbParam("LockoutEnabled", DbType.Boolean, lockoutEnabled),
                        new DbParam("AccessFailedCount", DbType.Int32, accessFailedCount),
                        new DbParam("ResetToken", DbType.String, 255, resetToken.ToEmptyIfNull()),
                        new DbParam("ConfirmationToken", DbType.String, 255, confirmationToken.ToEmptyIfNull()),
                        new DbParam("ApiKey", DbType.String, 255, apiKey.ToEmptyIfNull()),
                        new DbParam("TimeZone", DbType.String, 255, timeZone.ToEmptyIfNull()),
                        new DbParam("ObserveDst", DbType.Boolean, observeDst),
                        new DbParam("Culture", DbType.String, 50, culture.ToEmptyIfNull()),
                        new DbParam("Theme", DbType.String, 255, theme.ToEmptyIfNull()),
                        new DbParam("Ipv4Address", DbType.String, 20, ipv4Address.ToEmptyIfNull()),
                        new DbParam("Ipv6Address", DbType.String, 50, ipv6Address.ToEmptyIfNull()),
                        new DbParam("Biography", DbType.String, 255, bio.ToEmptyIfNull()),
                        new DbParam("Location", DbType.String, 255, location.ToEmptyIfNull()),
                        new DbParam("Url", DbType.String, 255, url.ToEmptyIfNull()),
                        new DbParam("Visits", DbType.Int32, visits),
                        new DbParam("VisitsUpdatedDate", DbType.DateTimeOffset, visitsUpdatedDate.ToDateIfNull()),
                        new DbParam("minutesActive", DbType.Int32, minutesActive),
                        new DbParam("MinutesActiveUpdatedDate", DbType.DateTimeOffset,
                            minutesActiveUpdatedDate.ToDateIfNull()),
                        new DbParam("Reputation", DbType.Int32, reputation),
                        new DbParam("ReputationUpdatedDate", DbType.DateTimeOffset, reputationUpdatedDate.ToDateIfNull()),
                        new DbParam("Rank", DbType.Int32, rank),
                        new DbParam("RankUpdatedDate", DbType.DateTimeOffset, rankUpdatedDate.ToDateIfNull()),
                        new DbParam("Signature", DbType.String, signature.ToEmptyIfNull()),
                        new DbParam("SignatureHtml", DbType.String, signatureHtml.ToEmptyIfNull()),
                        new DbParam("IsSpam", DbType.Boolean, isSpam),
                        new DbParam("IsSpamUpdatedUserId", DbType.Int32, isSpamUpdatedUserId),
                        new DbParam("IsSpamUpdatedDate", DbType.DateTimeOffset, isSpamUpdatedDate),
                        new DbParam("IsVerified", DbType.Boolean, isVerified),
                        new DbParam("IsVerifiedUpdatedUserId", DbType.Int32, isVerifiedUpdatedUserId),
                        new DbParam("IsVerifiedUpdatedDate", DbType.DateTimeOffset, isVerifiedUpdatedDate),
                        new DbParam("IsBanned", DbType.Boolean, isBanned),
                        new DbParam("IsBannedUpdatedUserId", DbType.Int32, isBannedUpdatedUserId),
                        new DbParam("IsBannedUpdatedDate", DbType.DateTimeOffset, isBannedUpdatedDate),
                        new DbParam("IsBannedExpiryDate", DbType.DateTimeOffset, isBannedExpiryDate),
                        new DbParam("UserType", DbType.Int16, userType),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate.ToDateIfNull()),
                        new DbParam("LastLoginDate", DbType.DateTimeOffset, lastLoginDate.ToDateIfNull()),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output)
                    });
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