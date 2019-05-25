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
     
        private readonly IDbContext _dbContext;
        private readonly ILogger<UserBannerRepository> _logger;
        
        public UserBannerRepository(
            IDbContext dbContext,
            ILogger<UserBannerRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
     
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
        
        public Task<IPagedResults<UserBanner>> SelectAsync(DbParam[] dbParams)
        {
            throw new NotImplementedException();
        }
        
        public async Task<UserBanner> SelectByIdAsync(int id)
        {
            UserBanner banner = null;
            using (var context = _dbContext)
            {
                banner = await context.ExecuteReaderAsync2(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectUserBanner",
                    async reader =>
                    {
                        if ((reader != null) && reader.HasRows)
                        {
                            await reader.ReadAsync();
                            banner = new UserBanner(reader);
                        }

                        return banner;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

              
            }

            return banner;
        }

        public async Task<UserBanner> SelectByUserIdAsync(int userId)
        {
            UserBanner banner = null;
            using (var context = _dbContext)
            {
                banner = await context.ExecuteReaderAsync2(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectUserBannerByUserId",
                    async reader =>
                    {
                        if ((reader != null) && reader.HasRows)
                        {
                            await reader.ReadAsync();
                            banner = new UserBanner(reader);
                        }

                        return banner;
                    }, new[]
                    {
                        new DbParam("UserId", DbType.Int32, userId)
                    });

            }

            return banner;
        }

        // ----------------

        async Task<int> InsertUpdateInternal(
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
                bannerId = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUserBanner",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("UserId", DbType.Int32, userId),
                        new DbParam("Name", DbType.String, 255, name),
                        new DbParam("ContentBlob", DbType.Binary, contentBlob ?? new byte[0]),
                        new DbParam("ContentType", DbType.String, 75, contentType),
                        new DbParam("ContentLength", DbType.Int64, contentLength),
                        new DbParam("CreatedUserId", DbType.Int64, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int64, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }

            return bannerId;
        }

    }
}