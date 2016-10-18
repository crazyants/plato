using System;
using System.Collections.Generic;
using Plato.Data;
using System.Data;
using Plato.Models.User;
using Plato.Abstractions.Extensions;

namespace Plato.Repositories.Users
{

    public class UserRepository : IUserRepository<User>
    {

        #region "Private Variables"

        private IDbContextt _dbContext;
        private IUserSecretRepository<UserSecret> _userSecretRepository;
        private IUserDetailRepository<UserDetail> _userDetailRepository;
        private IUserPhotoRepository<UserPhoto> _userPhotoRepository;

        #endregion

        #region "Constructor"

        public UserRepository(
            IDbContextt dbContext,
            IUserSecretRepository<UserSecret> userSecretRepository,
            IUserDetailRepository<UserDetail> userDetailRepository,
            IUserPhotoRepository<UserPhoto> userPhotoRepository)
        {
            _dbContext = dbContext;
            _userSecretRepository = userSecretRepository;
            _userDetailRepository = userDetailRepository;
            _userPhotoRepository = userPhotoRepository;

        }

        #endregion

        #region "Implementation"

        public bool Dlete(int id)
        {
            throw new NotImplementedException();
        }

        public User InsertUpdate(User user)
        {

            if (_userSecretRepository == null)
                throw new ArgumentNullException(nameof(_userSecretRepository));

            if (_userDetailRepository == null)
                throw new ArgumentNullException(nameof(_userDetailRepository));

            int Id = InsertUpdateInternal(
                user.Id,
                user.SiteId,
                user.UserName,
                user.Email,
                user.DisplayName,
                user.SamAccountName);

            if (Id > 0)
            {

                // secerts

                if (user.Secret == null)
                    user.Secret = new UserSecret();
                if (user.Id == 0 || user.Secret.UserId == 0)
                    user.Secret.UserId = Id;
                _userSecretRepository.InsertUpdate(user.Secret);

                // detail

                if (user.Detail == null)
                    user.Detail = new UserDetail();
                if (user.Id == 0 || user.Detail.UserId == 0)
                    user.Detail.UserId = Id;
                _userDetailRepository.InsertUpdate(user.Detail);


                // photos

                if (user.Photo == null)
                    user.Photo = new UserPhoto();
                if (user.Id == 0 || user.Photo.UserId == 0)
                    user.Photo.UserId = Id;
                _userPhotoRepository.InsertUpdate(user.Photo);
             
                // return

                return SelectById(Id);

            }
            
            return null;     

        }

        public User SelectById(int Id)
        {

            User user = new User();
            using (var context = _dbContext)
            {
                IDataReader reader = context.ExecuteReader(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectUser", Id);

                if (reader != null)
                {
                    reader.Read();   
                    user.PopulateModel(reader);

                    if (reader.NextResult())
                    {
                        reader.Read();
                        user.Secret = new UserSecret();
                        user.Secret.PopulateModel(reader);
                    }
                    

                    if (reader.NextResult())
                    {
                        reader.Read();
                        user.Detail = new UserDetail();
                        user.Detail.PopulateModel(reader);
                    }

                    if (reader.NextResult())
                    {
                        reader.Read();
                        user.Photo = new UserPhoto();
                        user.Photo.PopulateModel(reader);
                    }

                }
            }

            return user;

        }

        public IEnumerable<User> SelectPaged(int pageIndex, int pageSize, object options)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Private Methods"

        private int InsertUpdateInternal(
            int Id,
            int SiteId,
            string Name,
            string EmailAddress,
            string DisplayName,
            string SamAccountName)
        {

            int id = 0;
            using (var context = _dbContext)
            {

                id = context.ExecuteScalar<int>(
                  CommandType.StoredProcedure,
                  "plato_sp_InsertUpdateUser",
                    Id,
                    SiteId,
                    Name.ToEmptyIfNull(),
                    EmailAddress.ToEmptyIfNull(),
                    DisplayName.ToEmptyIfNull(),
                    SamAccountName.ToEmptyIfNull());

            }
                       
            return id;

        }
        
        #endregion

    }
}
