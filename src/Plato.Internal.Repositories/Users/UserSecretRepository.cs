using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Repositories.Users
{
    public class UserSecretRepository : IUserSecretRepository<UserSecret>
    {
        #region "Constructor"

        public UserSecretRepository(
            IDbContext dbContext,
            ILogger<UserSecretRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<UserSecretRepository> _logger;

        #endregion

        #region "Implementation"
        
        public Task<bool> DeleteAsync(int id)
        {
            // TODO
            throw new NotImplementedException();
        }

        public async Task<UserSecret> InsertUpdateAsync(UserSecret secret)
        {
            var id = await InsertUpdateInternal(
                secret.Id,
                secret.UserId,
                secret.Secret,
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
                secret = await context.ExecuteReaderAsync2(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectUserSecret",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            secret = new UserSecret();
                            await reader.ReadAsync();
                            secret.PopulateModel(reader);
                        }

                        return secret;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

              
            }

            return secret;
        }
        
        public Task<IPagedResults<UserSecret>> SelectAsync(DbParam[] dbParams)
        {
            // TODO
            throw new NotImplementedException();
        }
        
        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int userId,
            string secret,
            int[] salts,
            string securityStamp)
        {
            string delimitedSalts = null;
            if (salts != null)
                delimitedSalts = salts.ToDelimitedString();

            var dbId = 0;
            using (var context = _dbContext)
            {
                dbId = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "plato_sp_InsertUpdateUserSecret",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("UserId", DbType.Int32, userId),
                        new DbParam("Secret", DbType.String, 255, secret),
                        new DbParam("DelimitedSalts", DbType.String, 255, delimitedSalts),
                        new DbParam("SecurityStamp", DbType.String, 255, securityStamp),
                    });
            }

            return dbId;
        }



        #endregion
    }
}