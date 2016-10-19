using System;
using System.Collections.Generic;
using Plato.Data;
using System.Data;
using Plato.Models.Users;
using Plato.Abstractions.Extensions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Plato.Repositories.Users
{
    public class UserPhotoRepository : IUserPhotoRepository<UserPhoto>
    {

        #region "Private Variables"

        private IDbContextt _dbContext;
        private ILogger<UserSecretRepository> _logger;

        #endregion

        #region "Constructor"

        public UserPhotoRepository(
            IDbContextt dbContext,
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

        public async Task<UserPhoto> InsertUpdate(UserPhoto photo)
        {

            int id = InsertUpdateInternal(
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
            {               
                return await SelectById(id);
            }
                

            return null;


        }
         

        public async Task<UserPhoto> SelectById(int Id)
        {
            UserPhoto photo = null;
            using (var context = _dbContext)
            {
                IDataReader reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectUserPhoto", Id);

                if (reader != null)
                {
                    reader.Read();
                    photo = new UserPhoto();
                    photo.PopulateModel(reader);
                }
            }

            return photo;
        }

        public Task<IEnumerable<UserPhoto>> SelectPaged(int pageIndex, int pageSize, object options)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region "Private Methods"

        private int InsertUpdateInternal(
            int Id,
            int UserId,
            string Name,
            string BackColor,
            string ForeColor,
            byte[] ContentBlob,
            string ContentType,
            float ContentLength,     
            DateTime? CreatedDate,
            int CreatedUserId,
            DateTime? ModifiedDate,
            int ModifiedUserId)
        {

       
            int id = 0;
            using (var context = _dbContext)
            {

                id = context.ExecuteScalar<int>(
                  CommandType.StoredProcedure,
                  "plato_sp_InsertUpdateUserPhoto",
                    Id,
                    UserId,
                    Name.ToEmptyIfNull(),
                    BackColor.ToEmptyIfNull(),
                    ForeColor.ToEmptyIfNull(),
                    ContentBlob ?? new byte[0],
                    ContentType.ToEmptyIfNull(),
                    ContentLength,             
                    CreatedDate,
                    CreatedUserId,
                    ModifiedDate,
                    ModifiedUserId);

            }

            return id;

        }

        #endregion

    }
}
