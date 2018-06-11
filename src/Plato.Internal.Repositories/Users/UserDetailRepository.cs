using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Abstractions.Extensions;
using Plato.Internal.Models.Users;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Repositories.Users
{
    public class UserDetailRepository : IUserDetailRepository<UserDetail>
    {
        #region "Constructor"

        public UserDetailRepository(
            IDbContext dbContext,
            ILogger<UserSecretRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int Id,
            int userId,
            int editionId,
            int roleId,
            int teamId,
            double timeZoneOffSet,
            bool observeDst,
            string culture,
            string firstName,
            string lastName,
            string webSiteUrl,
            string apiKey,
            int visits,
            int answers,
            int entities,
            int replies,
            int reactions,
            int mentions,
            int follows,
            int badges,
            int reputationRank,
            int reputationPoints,
            short photoId,
            short bannerId,
            string clientIpAddress,
            string clientName,
            string emailConfirmationCode,
            string passwordResetCode,
            bool isEmailConfirmed,
            DateTime? createdDate,
            int createdUserId,
            DateTime? modifiedDate,
            int modifiedUserId,
            bool isDeleted,
            DateTime? deletedDate,
            int deletedUserId,
            bool isBanned,
            DateTime? bannedDate,
            int bannedUserId,
            bool isLocked,
            DateTime? lockedDate,
            int lockedUserId,
            DateTime? unLockDate,
            bool isSpam,
            DateTime? spamDate,
            int spamUserId,
            DateTime? lastLoginDate
        )
        {
            var id = 0;
            using (var context = _dbContext)
            {
                id = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "plato_sp_InsertUpdateUserDetail",
                    Id,
                    userId,
                    editionId,
                    roleId,
                    teamId,
                    timeZoneOffSet,
                    observeDst,
                    culture.ToEmptyIfNull().TrimToSize(50),
                    firstName.ToEmptyIfNull().TrimToSize(100),
                    lastName.ToEmptyIfNull().TrimToSize(100),
                    webSiteUrl.ToEmptyIfNull().TrimToSize(100),
                    apiKey.ToEmptyIfNull().TrimToSize(255),
                    visits,
                    answers,
                    entities,
                    replies,
                    reactions,
                    mentions,
                    follows,
                    badges,
                    reputationRank,
                    reputationPoints,
                    photoId,
                    bannerId,
                    clientIpAddress.ToEmptyIfNull().TrimToSize(255),
                    clientName.ToEmptyIfNull().TrimToSize(255),
                    emailConfirmationCode.ToEmptyIfNull().TrimToSize(255),
                    passwordResetCode.ToEmptyIfNull().TrimToSize(255),
                    isEmailConfirmed,
                    createdDate,
                    createdUserId,
                    modifiedDate,
                    modifiedUserId,
                    isDeleted,
                    deletedDate,
                    deletedUserId,
                    isBanned,
                    bannedDate,
                    bannedUserId,
                    isLocked,
                    lockedDate,
                    lockedUserId,
                    unLockDate,
                    isSpam,
                    spamDate,
                    spamUserId,
                    lastLoginDate);
            }

            return id;
        }

        #endregion

        #region "Private Variables"

        private readonly IDbContext _dbContext;
        private ILogger<UserSecretRepository> _logger;

        #endregion

        #region "Implementation"

        public Task<bool> DeleteAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDetail> InsertUpdateAsync(UserDetail detail)
        {
            var id = await InsertUpdateInternal(
                detail.Id,
                detail.UserId,
                detail.EditionId,
                detail.RoleId,
                detail.TeamId,
                detail.TimeZoneOffSet,
                detail.ObserveDST,
                detail.Culture,
                detail.FirstName,
                detail.LastName,
                detail.WebSiteUrl,
                detail.ApiKey,
                detail.Visits,
                detail.Answers,
                detail.Entities,
                detail.Replies,
                detail.Reactions,
                detail.Mentions,
                detail.Follows,
                detail.Badges,
                detail.ReputationRank,
                detail.ReputationPoints,
                detail.PhotoId,
                detail.BannerId,
                detail.ClientIpAddress,
                detail.ClientName,
                detail.EmailConfirmationCode,
                detail.PasswordResetCode,
                detail.IsEmailConfirmed,
                detail.CreatedDate,
                detail.CreatedUserId,
                detail.ModifiedDate,
                detail.ModifiedUserId,
                detail.IsDeleted,
                detail.DeletedDate,
                detail.DeletedUserId,
                detail.IsBanned,
                detail.BannedDate,
                detail.BannedUserId,
                detail.IsLocked,
                detail.LockedDate,
                detail.LockedUserId,
                detail.UnLockDate,
                detail.IsSpam,
                detail.SpamDate,
                detail.SpamUserId,
                detail.LastLoginDate);

            if (id > 0)
                return await SelectByIdAsync(id);

            return null;
        }


        public async Task<UserDetail> SelectByIdAsync(int Id)
        {
            UserDetail detail = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectUserDetail", Id);

                if (reader != null)
                {
                    await reader.ReadAsync();
                    detail = new UserDetail();
                    detail.PopulateModel(reader);
                }
            }

            return detail;
        }
        
   
        public Task<IPagedResults<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}