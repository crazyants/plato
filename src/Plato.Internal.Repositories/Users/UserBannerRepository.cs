using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Repositories.Users
{
    public class UserBannerRepository : IUserBannerRepository<UserBanner>
    {
        #region "Constructor"

        public UserBannerRepository(
            IDbContext dbContext,
            ILogger<UserBannerRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int userId,
            string name,
            byte[] contentBlob,
            string contentType,
            float contentLength,
            DateTimeOffset? createdDate,
            int createdUserId,
            DateTimeOffset? modifiedDate,
            int modifiedUserId)
        {
            var bannerId = 0;
            using (var context = _dbContext)
            {
                bannerId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUserBanner",
                    id,
                    userId,
                    name.ToEmptyIfNull().TrimToSize(255),
                    contentBlob ?? new byte[0], // don't allow nulls so we can determine parameter type
                    contentType.ToEmptyIfNull().TrimToSize(75),
                    contentLength,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    modifiedUserId,
                    modifiedDate,
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }

            return bannerId;
        }

        #endregion

        #region "Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<UserBannerRepository> _logger;

        #endregion

        #region "Implementation"

        public Task<bool> DeleteAsync(int id)
        {
            // TODO
            throw new NotImplementedException();
        }

        public async Task<UserBanner> InsertUpdateAsync(UserBanner banner)
        {
            var id = await InsertUpdateInternal(
                banner.Id,
                banner.UserId,
                banner.Name,
                banner.ContentBlob,
                banner.ContentType,
                banner.ContentLength,
                banner.CreatedDate,
                banner.CreatedUserId,
                banner.ModifiedDate,
                banner.ModifiedUserId);

            if (id > 0)
                return await SelectByIdAsync(id);

            return null;
        }


        public Task<IPagedResults<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<UserBanner>> SelectAsync(params object[] inputParams)
        {
            throw new NotImplementedException();
        }


        public async Task<UserBanner> SelectByIdAsync(int id)
        {
            UserBanner banner = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectUserBanner", id);

                if ((reader != null) && reader.HasRows)
                {
                    await reader.ReadAsync();
                    banner = new UserBanner(reader);
                }
            }

            return banner;
        }

        public async Task<UserBanner> SelectByUserIdAsync(int userId)
        {
            UserBanner banner = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectUserBannerByUserId", userId);

                if ((reader != null) && reader.HasRows)
                {
                    await reader.ReadAsync();
                    banner = new UserBanner(reader);
                }
            }

            return banner;
        }

        #endregion
    }
}