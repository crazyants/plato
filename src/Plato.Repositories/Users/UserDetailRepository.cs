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
    public class UserDetailRepository : IUserDetailRepository<UserDetail>
    {

        #region "Private Variables"

        private IDbContext _dbContext;
        private ILogger<UserSecretRepository> _logger;

        #endregion

        #region "Constructor"

        public UserDetailRepository(
            IDbContext dbContext,
            ILogger<UserSecretRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public Task<bool> Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDetail> InsertUpdate(UserDetail detail)
        {

            int id = await InsertUpdateInternal(
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
                detail.Banner,
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
                return await SelectById(id);

            return null;

        }
         

        public async Task<UserDetail> SelectById(int Id)
        {
            UserDetail detail = null;
            using (var context = _dbContext)
            {
                DbDataReader reader = await context.ExecuteReaderAsync(
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

        public Task<IEnumerable<UserDetail>> SelectPaged(int pageIndex, int pageSize, object options)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Private Methods"
        
        private async Task<int> InsertUpdateInternal(
            int Id,     
            int UserId,
            int EditionId,
            int RoleId,
            int TeamId,
            double TimeZoneOffSet,
            bool ObserveDST,
            string Culture,
            string FirstName,
            string LastName,
            string WebSiteUrl,
            string ApiKey,
            int Visits,
            int Answers,
            int Entities,
            int Replies,
            int Reactions,
            int Mentions,
            int Follows,
            int Badges,
            int ReputationRank,
            int ReputationPoints,
            byte[] Banner,
            string ClientIpAddress,
            string ClientName,
            string EmailConfirmationCode,
            string PasswordResetCode,
            bool IsEmailConfirmed,
            DateTime? CreatedDate,
            int CreatedUserId,
            DateTime? ModifiedDate,
            int ModifiedUserId,
            bool IsDeleted,
            DateTime? DeletedDate,
            int DeletedUserId,
            bool IsBanned,
            DateTime? BannedDate,
            int BannedUserId,
            bool IsLocked,
            DateTime? LockedDate,
            int LockedUserId,
            DateTime? UnLockDate,
            bool IsSpam,
            DateTime? SpamDate,
            int SpamUserId,
            DateTime? LastLoginDate
            )
        {

            int id = 0;
            using (var context = _dbContext)
            {

                id = await context.ExecuteScalarAsync<int>(
                  CommandType.StoredProcedure,
                  "plato_sp_InsertUpdateUserDetail",
                    Id, 
                    UserId,                  
                    EditionId,
                    RoleId,
                    TeamId,
                    TimeZoneOffSet,
                    ObserveDST,
                    Culture.ToEmptyIfNull(),
                    FirstName.ToEmptyIfNull(),
                    LastName.ToEmptyIfNull(),
                    WebSiteUrl.ToEmptyIfNull(),
                    ApiKey.ToEmptyIfNull(),
                    Visits,
                    Answers,
                    Entities,
                    Replies,
                    Reactions,
                    Mentions,
                    Follows,
                    Badges,
                    ReputationRank,
                    ReputationPoints,
                    Banner ?? new byte[0],
                    ClientIpAddress.ToEmptyIfNull(),
                    ClientName.ToEmptyIfNull(),
                    EmailConfirmationCode.ToEmptyIfNull(),
                    PasswordResetCode.ToEmptyIfNull(),
                    IsEmailConfirmed,
                    CreatedDate,
                    CreatedUserId,
                    ModifiedDate,
                    ModifiedUserId,
                    IsDeleted,
                    DeletedDate,
                    DeletedUserId,
                    IsBanned,
                    BannedDate,
                    BannedUserId,
                    IsLocked,
                    LockedDate,
                    LockedUserId,
                    UnLockDate,
                    IsSpam,
                    SpamDate,
                    SpamUserId,
                    LastLoginDate);

            }
     
            return id;

        }

        #endregion



    }
}
