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

        private readonly IDbContext _dbContext;
        private readonly ILogger<UserSecretRepository> _logger;

        #endregion

        #region "Constructor"

        public UserSecretRepository(
            IDbContext dbContext,
            ILogger<UserSecretRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserSecret> InsertUpdateAsync(UserSecret secret)
        {
            
            var id = await InsertUpdateInternal(
                secret.Id,
                secret.UserId,
                secret.PasswordHash,
                secret.Salts,
                secret.SecurityStamp);

            if (id > 0)
                return await SelectByIdAsync(id);

            return null;

        }


        public async Task<UserSecret> SelectByIdAsync(int id)
        {
            UserSecret secret = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                  "plato_sp_SelectUserSecret", id);

                if (reader != null)
                {
                    await reader.ReadAsync();
                    secret = new UserSecret();
                    secret.PopulateModel(reader);
                }
            }

            return secret;

        }

        public Task<IEnumerable<UserSecret>> SelectPagedAsync(int pageIndex, int pageSize, object options)
        {
            throw new NotImplementedException();
        }

        #endregion
        
        #region "Private Methods"
        
        private async Task<int> InsertUpdateInternal(
            int id,
            int userId,
            string passwordHash,           
            int[] salts,
            string securityStamp)
        {

            string delimitedSalts = null;
            if (salts != null)
                delimitedSalts = salts.ToDelimitedString();
            
            var dbId = 0;
            using (var context = _dbContext)
            {

                dbId = await context.ExecuteScalarAsync<int>(
                  CommandType.StoredProcedure,
                  "plato_sp_InsertUpdateUserSecret",
                    id,
                    userId,
                    passwordHash.ToEmptyIfNull().TrimToSize(255),
                    delimitedSalts.ToEmptyIfNull().TrimToSize(255),
                    securityStamp.ToEmptyIfNull().TrimToSize(255));

            }
                     
            return dbId;

        }
        
        #endregion

    }
}
