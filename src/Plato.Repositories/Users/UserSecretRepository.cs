using System;
using System.Collections.Generic;
using Plato.Data;
using System.Data;
using Plato.Models.User;
using Plato.Abstractions.Extensions;


namespace Plato.Repositories.Users
{
    public class UserSecretRepository : IUserSecretRepository<UserSecret>
    {

        #region "Private Variables"

        private IDbContextt _dbContext;

        #endregion

        #region "Constructor"

        public UserSecretRepository(
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

        public UserSecret InsertUpdate(UserSecret secret)
        {
            
            int id = InsertUpdateInternal(
                secret.Id,
                secret.UserId,
                secret.Password,
                secret.Salts);

            if (id > 0)
                return SelectById(id);

            return null;


        }

        public UserSecret SelectById(int Id)
        {
            UserSecret secret = null;
            using (var context = _dbContext)
            {
                IDataReader reader = context.ExecuteReader(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectUserSecret", Id);

                if (reader != null)
                {
                    reader.Read();
                    secret = new UserSecret();
                    secret.PopulateModel(reader);
                }
            }

            return secret;
        }

        public IEnumerable<UserSecret> SelectPaged(int pageIndex, int pageSize, object options)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region "Private Methods"
        
        private int InsertUpdateInternal(
            int Id,
            int UserId,
            string Password,           
            int[] Salts)
        {

            string delimitedSalts = null;
            if (Salts != null)
                delimitedSalts = Salts.ToDelimitedString();
            
            int id = 0;
            using (var context = _dbContext)
            {

                id = context.ExecuteScalar<int>(
                  CommandType.StoredProcedure,
                  "plato_sp_InsertUpdateUserSecret",
                    Id,
                    UserId,
                    Password.ToEmptyIfNull(),
                    delimitedSalts.ToEmptyIfNull());

            }
                     
            return id;

        }
        
        #endregion

    }
}
