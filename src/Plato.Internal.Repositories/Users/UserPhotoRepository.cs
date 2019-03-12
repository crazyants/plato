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
     
        public Task<IPagedResults<UserPhoto>> SelectAsync(params object[] inputParams)
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
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteUserPhotoById", id);
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
                photo = await context.ExecuteReaderAsync<UserPhoto>(
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
                    },
                    id);


            }

            return photo;
        }

        public async Task<UserPhoto> SelectByUserIdAsync(int userId)
        {
            UserPhoto photo = null;
            using (var context = _dbContext)
            {
                photo = await context.ExecuteReaderAsync<UserPhoto>(
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
                    },
                    userId);


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
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUserPhoto",
                    id,
                    userId,
                    name.ToEmptyIfNull().ToSafeFileName().TrimToSize(255),
                    contentBlob ?? new byte[0], // don't allow nulls so we can determine parameter type
                    contentType.ToEmptyIfNull().TrimToSize(75),
                    contentLength,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    modifiedUserId,
                    modifiedDate,
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }
        }

        #endregion


    }
}