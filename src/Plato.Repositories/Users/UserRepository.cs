using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Data;
using System.Data;
using Plato.Repositories.Models;

namespace Plato.Repositories.Users
{
    
    public class UserRepository : IRepository<User>
    {

        #region "Private Variables"

        private IDbContextt _dbContext;

        #endregion
        
        #region "Constructor"

        public UserRepository(IDbContextt dbContext)
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

            using (var context = new DbContext("connectionstring"))
            {

                try
                {
                    IDataReader reader = context.ExecuteReader(
                      CommandType.StoredProcedure,
                      "iasp_sp_SelectUserData",
                      new
                      {
                          @strUsername = "",
                          @strPassword = "",
                          @strEncyptedPassword = "",
                          @intLoginUsing = 1,
                          @intUserID = 1
                      });


                    if (reader != null)
                    {
                        User.PopulateModelFromDataReader(reader);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
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
