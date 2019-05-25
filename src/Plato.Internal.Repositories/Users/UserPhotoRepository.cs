using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Repositories.Users
{
    public class UserPhotoRepository : IUserPhotoRepository<UserPhoto>
    {
        #region "Constructor"

        public UserPhotoRepository(
            IDbContext dbContext,
            ILogger<UserPhotoRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion
        
        #region "Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<UserPhotoRepository> _logger;

        #endregion

        #region "Implementation"
     
        public Task<IPagedResults<UserPhoto>> SelectAsync(DbParam[] dbParams)
        {
            // TODO
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting user photo with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteUserPhotoById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }

        public async Task<UserPhoto> InsertUpdateAsync(UserPhoto photo)
        {
            var id = await InsertUpdateInternal(
                photo.Id,
                photo.UserId,
                photo.Name,
                photo.ContentBlob,
                photo.ContentType,
                photo.ContentLength,
                photo.CreatedUserId,
                photo.CreatedDate,
                photo.ModifiedUserId,
                photo.ModifiedDate);

            if (id > 0)
                return await SelectByIdAsync(id);

            return null;
        }
   
        public async Task<UserPhoto> SelectByIdAsync(int id)
        {
            UserPhoto photo = null;
            using (var context = _dbContext)
            {
                photo = await context.ExecuteReaderAsync2<UserPhoto>(
                    CommandType.StoredProcedure,
                    "SelectUserPhotoById",
                    async reader =>
                    {
                        if ((reader != null) && reader.HasRows)
                        {
                            await reader.ReadAsync();
                            photo = new UserPhoto(reader);
                        }

                        return photo;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });


            }

            return photo;
        }

        public async Task<UserPhoto> SelectByUserIdAsync(int userId)
        {
            UserPhoto photo = null;
            using (var context = _dbContext)
            {
                photo = await context.ExecuteReaderAsync2<UserPhoto>(
                    CommandType.StoredProcedure,
                    "SelectUserPhotoByUserId",
                    async reader =>
                    {
                        if ((reader != null) && reader.HasRows)
                        {
                            await reader.ReadAsync();
                            photo = new UserPhoto(reader);
                        }

                        return photo;
                    }, new[]
                    {
                        new DbParam("UserId", DbType.Int32, userId)
                    });


            }

            return photo;
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
            int createdUserId,
            DateTimeOffset? createdDate,
            int modifiedUserId,
            DateTimeOffset? modifiedDate)
        {
            using (var context = _dbContext)
            {
                return await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUserPhoto",
                    new []
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("UserId", DbType.Int32, userId),
                        new DbParam("Name", DbType.String, 255, name.ToSafeFileName()),
                        new DbParam("ContentBlob", DbType.Binary, contentBlob ?? new byte[0]),
                        new DbParam("ContentType", DbType.String, 75, contentType),
                        new DbParam("ContentLength", DbType.Int64, contentLength),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset,  createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset,  modifiedDate),
                        new DbParam("UniqueId", DbType.Int32,  ParameterDirection.Output),
                    });
            }
        }

        #endregion


    }
}