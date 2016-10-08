using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Data;
using System.Data;
using Plato.Models.User;

namespace Plato.Repositories
{
    
    public class UserRepository : IUserRepository<User>
    {

        #region "Private Variables"

        private IDbContextt _dbContext;

        #endregion
        
        #region "Constructor"

        public UserRepository(
            IDbContextt dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region "Implementation"

        public bool Dlete(int id)
        {
            throw new NotImplementedException();
        }

        public int InsertUpdate(User entity)
        {
            throw new NotImplementedException();
        }

        public User Select(int id)
        {
            User User = new User();

            using (var context = _dbContext)
            {

                IDataReader reader = context.ExecuteReader(
                  CommandType.StoredProcedure,
                  "iasp_sp_SelectUserData",                
                      "",
                      "",
                      "",
                      1,
                      1                 
                  );


                //var p = new List<DbParameter>();
                //p.Add(new DbParameter("strUsername", ""));
                //p.Add(new DbParameter("strPassword", ""));
                //p.Add(new DbParameter("strEncyptedPassword", ""));
                //p.Add(new DbParameter("intLoginUsing", 1));
                //p.Add(new DbParameter("intUserID", 1));

                //IDataReader reader = context.ExecuteReader(
                //     CommandType.StoredProcedure,
                //     "iasp_sp_SelectUserData", p);       

                if (reader != null)
                {
                    reader.Read();
                    User.PopulateModelFromDataReader(reader);
                }
                                
            }

            return User;

        }

        public IEnumerable<User> SelectPaged(int pageIndex, int pageSize, object options)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Private Methods"



        #endregion

    }
}
