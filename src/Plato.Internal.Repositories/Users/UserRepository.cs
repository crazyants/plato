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
                (short) user.UserType,
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
                user = await context.ExecuteReaderAsync<User>(
                    CommandType.StoredProcedure,
                    "SelectUserById",
                    async reader => await BuildUserFromResultSets(reader),
                    id);
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
                user = await context.ExecuteReaderAsync2(
                    CommandType.StoredProcedure,
                    "SelectUserByUserNameNormalized",
                    async reader => await BuildUserFromResultSets(reader),
                    new []
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
                    email.TrimToSize(255));
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
                user = await context.ExecuteReaderAsync<User>(
                    CommandType.StoredProcedure,
                    "SelectUserByEmailNormalized",
                    async reader => await BuildUserFromResultSets(reader),
                    emailNormalized.TrimToSize(255));
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
                user = await context.ExecuteReaderAsync2<User>(
                    CommandType.StoredProcedure,
                    "SelectUserByUserName",
                    async reader => await BuildUserFromResultSets(reader),
                    new[]
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
                user = await context.ExecuteReaderAsync2<User>(
                    CommandType.StoredProcedure,
                    "SelectUserByUserNameAndPassword",
                    async reader => await BuildUserFromResultSets(reader),
                    new[]
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
                user = await context.ExecuteReaderAsync2(
                    CommandType.StoredProcedure,
                    "SelectUserByEmailAndPassword",
                    async reader => await BuildUserFromResultSets(reader),
                    new[]
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
                user = await context.ExecuteReaderAsync2<User>(
                    CommandType.StoredProcedure,
                    "SelectUserByResetToken",
                    async reader => await BuildUserFromResultSets(reader),
                    new[]
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
                user = await context.ExecuteReaderAsync2<User>(
                    CommandType.StoredProcedure,
                    "SelectUserByConfirmationToken",
                    async reader => await BuildUserFromResultSets(reader),
                    new[]
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
                user = await context.ExecuteReaderAsync2<User>(
                    CommandType.StoredProcedure,
                    "SelectUserByApiKey",
                    async reader => await BuildUserFromResultSets(reader),
                    new[]
                    {
                        new DbParam("ApiKey", DbType.String, 255, apiKey)
                    });
            }

            return user;

        }
        
        public async Task<IPagedResults<User>> SelectAsync(params object[] inputParams)
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
                    inputParams);
              
            }

            return results;

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

                userId = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUser",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("PrimaryRoleId", DbType.Int32, primaryRoleId),
                        new DbParam("UserName", DbType.String, 255, userName),
                        new DbParam("NormalizedUserName", DbType.String, 255, normalizedUserName),
                        new DbParam("Email", DbType.String, 255, email),
                        new DbParam("NormalizedEmail", DbType.String, 255, normalizedEmail),
                        new DbParam("EmailConfirmed", DbType.Boolean, emailConfirmed),
                        new DbParam("DisplayName", DbType.String, 255, displayName),
                        new DbParam("FirstName", DbType.String, 255, firstName),
                        new DbParam("LastName", DbType.String, 255, lastName),
                        new DbParam("Alias", DbType.String, 255, alias),
                        new DbParam("PhotoUrl", DbType.String, 255, photoUrl),
                        new DbParam("PhotoColor", DbType.String, 6, photoColor),
                        new DbParam("SamAccountName", DbType.String, 255, samAccountName),
                        new DbParam("PasswordHash", DbType.String, 255, passwordHash),
                        new DbParam("PasswordExpiryDate", DbType.DateTimeOffset, passwordExpiryDate),
                        new DbParam("PasswordUpdatedDate", DbType.DateTimeOffset, passwordUpdatedDate),
                        new DbParam("SecurityStamp", DbType.String, 255, securityStamp),
                        new DbParam("PhoneNumber", DbType.String, 255, phoneNumber),
                        new DbParam("PhoneNumberConfirmed", DbType.Boolean, phoneNumberConfirmed),
                        new DbParam("TwoFactorEnabled", DbType.Boolean, twoFactorEnabled),
                        new DbParam("LockoutEnd", DbType.DateTimeOffset, lockoutEnd),
                        new DbParam("LockoutEnabled", DbType.Boolean, lockoutEnabled),
                        new DbParam("AccessFailedCount", DbType.Int32, accessFailedCount),
                        new DbParam("ResetToken", DbType.String, 255, resetToken),
                        new DbParam("ConfirmationToken", DbType.String, 255, confirmationToken),
                        new DbParam("ApiKey", DbType.String, 255, apiKey),
                        new DbParam("TimeZone", DbType.String, 255, timeZone),
                        new DbParam("ObserveDst", DbType.Boolean, observeDst),
                        new DbParam("Culture", DbType.String, 50, culture),
                        new DbParam("Theme", DbType.String, 255, theme),
                        new DbParam("Ipv4Address", DbType.String, 20, ipv4Address),
                        new DbParam("Ipv6Address", DbType.String, 50, ipv6Address),
                        new DbParam("Biography", DbType.String, 255, bio),
                        new DbParam("Location", DbType.String, 255, location),
                        new DbParam("Url", DbType.String, 255, url),
                        new DbParam("Visits", DbType.Int32, visits),
                        new DbParam("VisitsUpdatedDate", DbType.DateTimeOffset, visitsUpdatedDate.ToDateIfNull()),
                        new DbParam("minutesActive", DbType.Int32, minutesActive),
                        new DbParam("MinutesActiveUpdatedDate", DbType.DateTimeOffset,
                            minutesActiveUpdatedDate.ToDateIfNull()),
                        new DbParam("Reputation", DbType.Int32, reputation),
                        new DbParam("ReputationUpdatedDate", DbType.DateTimeOffset,
                            reputationUpdatedDate.ToDateIfNull()),
                        new DbParam("Rank", DbType.Int32, rank),
                        new DbParam("RankUpdatedDate", DbType.DateTimeOffset, rankUpdatedDate.ToDateIfNull()),
                        new DbParam("Signature", DbType.String, signature),
                        new DbParam("SignatureHtml", DbType.String, signatureHtml),
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