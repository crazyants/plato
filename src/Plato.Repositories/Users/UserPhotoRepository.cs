using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Abstractions.Collections;
using Plato.Abstractions.Extensions;
using Plato.Data;
using Plato.Data.Abstractions;
using Plato.Models.Users;

namespace Plato.Repositories.Users
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

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int userId,
            string name,
            byte[] contentBlob,
            string contentType,
            float contentLength,
            DateTime? createdDate,
            int createdUserId,
            DateTime? modifiedDate,
            int modifiedUserId)
        {
            using (var context = _dbContext)
            {
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "plato_sp_InsertUpdateUserPhoto",
                    id,
                    userId,
                    name.ToEmptyIfNull().TrimToSize(255),
                    contentBlob ?? new byte[0], // don't allow nulls so we can determine parameter type
                    contentType.ToEmptyIfNull().TrimToSize(75),
                    contentLength,
                    createdDate,
                    createdUserId,
                    modifiedDate,
                    modifiedUserId);
            }
        }

        #endregion

        #region "Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<UserPhotoRepository> _logger;

        #endregion

        #region "Implementation"

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
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
                photo.CreatedDate,
                photo.CreatedUserId,
                photo.ModifiedDate,
                photo.ModifiedUserId);

            if (id > 0)
                return await SelectByIdAsync(id);

            return null;
        }


        public Task<IPagedResults<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class
        {
            throw new NotImplementedException();
        }

        public async Task<UserPhoto> SelectByIdAsync(int id)
        {
            UserPhoto photo = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectUserPhoto", id);

                if ((reader != null) && reader.HasRows)
                {
                    await reader.ReadAsync();
                    photo = new UserPhoto(reader);
                }
            }

            return photo;
        }

        public async Task<UserPhoto> SelectByUserIdAsync(int userId)
        {
            UserPhoto photo = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectUserPhotoByUserId", userId);

                if ((reader != null) && reader.HasRows)
                {
                    await reader.ReadAsync();
                    photo = new UserPhoto(reader);
                }
            }

            return photo;
        }

        #endregion
    }
}