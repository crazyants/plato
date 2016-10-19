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

    //public class UserBuilder
    //{

    //    public static User Build(IDataReader dr)
    //    {

    //        User user = new User()
    //        {
    //            Id = Convert.ToInt32(dr["Id"]),
    //            UserName = (dr.ColumnIsNotNull("UserName") ? Convert.ToString(dr["UserName"]) : string.Empty),
    //            Email = (dr.ColumnIsNotNull("Email") ? Convert.ToString(dr["Email"]) : string.Empty),
    //            DisplayName = (dr.ColumnIsNotNull("DisplayName") ? Convert.ToString(dr["DisplayName"]) : string.Empty)

    //        };

    //        if (dr.NextResult())
    //        {
    //            dr.Read();
    //            user.Secret = new UserSecret();
    //            user.Secret.PopulateModel(dr);
    //        }


    //        if (dr.NextResult())
    //        {
    //            dr.Read();
    //            user.Detail = new UserDetail();
    //            user.Detail.PopulateModel(dr);
    //        }

    //        if (dr.NextResult())
    //        {
    //            dr.Read();
    //            user.Photo = new UserPhoto();
    //            user.Photo.PopulateModel(dr);
    //        }


    //        return user;

    //    }


    //}

    public class UserRepository : IUserRepository<User>
    {

        #region "Private Variables"

        private IDbContextt _dbContext;
        private IUserSecretRepository<UserSecret> _userSecretRepository;
        private IUserDetailRepository<UserDetail> _userDetailRepository;
        private IUserPhotoRepository<UserPhoto> _userPhotoRepository;
        private ILogger<UserSecretRepository> _logger;

        #endregion

        #region "Constructor"

        public UserRepository(
            IDbContextt dbContext,
            IUserSecretRepository<UserSecret> userSecretRepository,
            IUserDetailRepository<UserDetail> userDetailRepository,
            IUserPhotoRepository<UserPhoto> userPhotoRepository,
            ILogger<UserSecretRepository> logger)
        {

            _dbContext = dbContext;
            _userSecretRepository = userSecretRepository;
            _userDetailRepository = userDetailRepository;
            _userPhotoRepository = userPhotoRepository;
            _logger = logger;

        }

        #endregion

        #region "Implementation"

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<User> InsertUpdate(User user)
        {

            if (_userSecretRepository == null)
                throw new ArgumentNullException(nameof(_userSecretRepository));

            if (_userDetailRepository == null)
                throw new ArgumentNullException(nameof(_userDetailRepository));

            int id = InsertUpdateInternal(
                user.Id,
                user.SiteId,
                user.UserName,
                user.Email,
                user.DisplayName,
                user.SamAccountName);

            if (id > 0)
            {

                // secerts

                if (user.Secret == null)
                    user.Secret = new UserSecret();
                if (user.Id == 0 || user.Secret.UserId == 0)
                    user.Secret.UserId = id;
                await _userSecretRepository.InsertUpdate(user.Secret);

                // detail

                if (user.Detail == null)
                    user.Detail = new UserDetail();
                if (user.Id == 0 || user.Detail.UserId == 0)
                    user.Detail.UserId = id;
                await _userDetailRepository.InsertUpdate(user.Detail);
                
                // photos

                if (user.Photo == null)
                    user.Photo = new UserPhoto();
                if (user.Id == 0 || user.Photo.UserId == 0)
                    user.Photo.UserId = id;
                await _userPhotoRepository.InsertUpdate(user.Photo);
             
                // return

                return await SelectById(id);

            }

            return null;    

        }
        
        public async Task<User> SelectById(int Id)
        {

            User user = new User();
            using (var context = _dbContext)
            {
                IDataReader reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectUser", Id);
                  //.Select(u => UserBuilder.Build(reader));

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

        public Task<IEnumerable<User>> SelectPaged(int pageIndex, int pageSize, object options)
        {

            return null;
           
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
