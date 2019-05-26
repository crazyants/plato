using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Roles;

namespace Plato.Internal.Repositories.Roles
{
    public class RoleRepository : IRoleRepository<Role>
    {

        #region "Constructor"

        private readonly IDbContext _dbContext;
        private readonly ILogger<RoleRepository> _logger;

        public RoleRepository(
            IDbContext dbContext,
            ILogger<RoleRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion
        
        #region "Implementation"

        public async Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting role with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteRoleById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<Role> InsertUpdateAsync(Role role)
        {

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var claims = "";
            if (role.RoleClaims != null)
            {
                claims = role.RoleClaims.Serialize();
            }

            var id = await InsertUpdateInternal(
                role.Id,
                role.Name,
                role.NormalizedName,
                role.Description,
                claims,
                role.CreatedDate,
                role.CreatedUserId,
                role.ModifiedDate,
                role.ModifiedUserId,
                role.ConcurrencyStamp);

            if (id > 0)
                return await SelectByIdAsync(id);

            return null;
        }

        public async Task<Role> SelectByIdAsync(int id)
        {
            Role role = null;
            using (var context = _dbContext)
            {
                role = await context.ExecuteReaderAsync2<Role>(
                    CommandType.StoredProcedure,
                    "SelectRoleById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            role = new Role();
                            await reader.ReadAsync();
                            role.PopulateModel(reader);
                        }

                        return role;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

            }

            return role;

        }

        public async Task<Role> SelectByNameAsync(string name)
        {
            Role role = null;
            using (var context = _dbContext)
            {
                role = await context.ExecuteReaderAsync2(
                    CommandType.StoredProcedure,
                    "SelectRoleByName",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            role = new Role();
                            await reader.ReadAsync();
                            role.PopulateModel(reader);
                        }

                        return role;
                    }, new[]
                    {
                        new DbParam("Name", DbType.String, 255, name)
                    });


            }

            return role;
        }

        public async Task<Role> SelectByNormalizedNameAsync(string normalizedName)
        {
            Role role = null;
            using (var context = _dbContext)
            {
                role = await context.ExecuteReaderAsync2<Role>(
                    CommandType.StoredProcedure,
                    "SelectRoleByNameNormalized",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            role = new Role();
                            await reader.ReadAsync();
                            role.PopulateModel(reader);
                        }

                        return role;
                    }, new[]
                    {
                        new DbParam("NormalizedName", DbType.String, 255, normalizedName),
                    });


            }

            return role;
        }

        public async Task<IList<Role>> SelectByUserIdAsync(int userId)
        {
            List<Role> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync2(
                    CommandType.StoredProcedure,
                    "SelectRolesByUserId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<Role>();
                            while (await reader.ReadAsync())
                            {
                                var role = new Role();
                                role.PopulateModel(reader);
                                output.Add(role);
                            }

                        }

                        return output;
                    }, new []
                    {
                        new DbParam("UserId", DbType.Int32, userId)
                    });

            }

            return output;

        }

        public async Task<IPagedResults<Role>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<Role> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync2(
                    CommandType.StoredProcedure,
                    "SelectRolesPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<Role>();
                            while (await reader.ReadAsync())
                            {
                                var role = new Role();
                                role.PopulateModel(reader);
                                output.Data.Add(role);
                            }

                            if (await reader.NextResultAsync())
                            {
                                if (reader.HasRows)
                                {
                                    await reader.ReadAsync();
                                    output.PopulateTotal(reader);
                                }
                          
                            }
                        }

                        return output;
                    },
                    dbParams);

              
            }

            return output;
        }


        public async Task<IEnumerable<Role>> SelectRoles()
        {
            IList<Role> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync2(
                    CommandType.StoredProcedure,
                    "SelectRoles",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<Role>();
                            while (await reader.ReadAsync())
                            {
                                var role = new Role();
                                role.PopulateModel(reader);
                                output.Add(role);
                            }
                        }

                        return output;
                    });
                
            }

            return output;

        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            string name,
            string nameNormalized,
            string description,
            string claims,
            DateTimeOffset? createdDate,
            int createdUserId,
            DateTimeOffset? modifiedDate,
            int modifiedUserId,
            string concurrencyStamp)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            using (var context = _dbContext)
            {
                return await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateRole",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("Name", DbType.String, 255, name),
                        new DbParam("NameNormalized", DbType.String, 255, nameNormalized),
                        new DbParam("Description", DbType.String, 255, description),
                        new DbParam("Claims", DbType.String, claims),
                        new DbParam("ConcurrencyStamp", DbType.String, 255, concurrencyStamp),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }
        }

        #endregion

    }

}