using System;
using System.Collections.Generic;
using Plato.Data;
using System.Data;
using Plato.Models.User;
using Plato.Abstractions.Extensions;

namespace Plato.Repositories.Users
{
    public class UserPhotoRepository : IUserPhotoRepository<UserPhoto>
    {

        #region "Private Variables"

        private IDbContextt _dbContext;

        #endregion

        #region "Constructor"

        public UserPhotoRepository(
            IDbContextt dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region "Implementation"

        public bool Dlete(int Id)
        {
            throw new NotImplementedException();
        }

        public UserPhoto InsertUpdate(UserPhoto photo)
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
                return SelectById(id);

            return null;


        }

        public UserPhoto SelectById(int Id)
        {
            UserPhoto photo = null;
            using (var context = _dbContext)
            {
                IDataReader reader = context.ExecuteReader(
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

        public IEnumerable<UserPhoto> SelectPaged(int pageIndex, int pageSize, object options)
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
