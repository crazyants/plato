using System;
using System.Collections.Generic;
using Plato.Data;
using System.Data;
using Plato.Models.Users;
using Plato.Abstractions.Extensions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using Plato.Abstractions.Collections;

namespace Plato.Repositories.Users
{
    public class UserPhotoRepository : IUserPhotoRepository<UserPhoto>
    {

        #region "Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<UserSecretRepository> _logger;

        #endregion

        #region "Constructor"

        public UserPhotoRepository(
            IDbContext dbContext,
            ILogger<UserSecretRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public Task<bool> DeleteAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserPhoto> InsertUpdateAsync(UserPhoto photo)
        {

            int id = await InsertUpdateInternal(
                photo.Id,
                photo.UserId,
                photo.Name,
                photo.BackColor,
                photo.ForeColor,
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

        public async Task<UserPhoto> SelectByIdAsync(int Id)
        {
            UserPhoto photo = null;
            using (var context = _dbContext)
            {
                DbDataReader reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectUserPhoto", Id);

                if (reader != null)
                {
                    await reader.ReadAsync();
                    photo = new UserPhoto();
                    photo.PopulateModel(reader);
                }
            }

            return photo;
        }
        
        #endregion
        
        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int userId,
            string name,
            string backColor,
            string foreColor,
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
                    backColor.ToEmptyIfNull().TrimToSize(20),
                    foreColor.ToEmptyIfNull().TrimToSize(20),
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

    }
}
