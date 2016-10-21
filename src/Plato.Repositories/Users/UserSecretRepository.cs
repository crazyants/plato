using System;
using System.Collections.Generic;
using Plato.Data;
using System.Data;
using Plato.Models.Users;
using Plato.Abstractions.Extensions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Plato.Repositories.Users
{
    public class UserSecretRepository : IUserSecretRepository<UserSecret>
    {

        #region "Private Variables"

        private IDbContextt _dbContext;
        private ILogger<UserSecretRepository> _logger;

        #endregion

        #region "Constructor"

        public UserSecretRepository(
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

        public async Task<UserSecret> InsertUpdate(UserSecret secret)
        {
            
            int id = await InsertUpdateInternal(
                secret.Id,
                secret.UserId,
                secret.Password,
                secret.Salts);

            if (id > 0)
                return await SelectById(id);

            return null;

        }


        public async Task<UserSecret> SelectById(int Id)
        {
            UserSecret secret = null;
            using (var context = _dbContext)
            {
                DbDataReader reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectUserSecret", Id);

                if (reader != null)
                {
                    await reader.ReadAsync();
                    secret = new UserSecret();
                    secret.PopulateModel(reader);
                }
            }

            return secret;

        }

        public Task<IEnumerable<UserSecret>> SelectPaged(int pageIndex, int pageSize, object options)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region "Private Methods"
        
        private async Task<int> InsertUpdateInternal(
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

                id = await context.ExecuteScalarAsync<int>(
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
