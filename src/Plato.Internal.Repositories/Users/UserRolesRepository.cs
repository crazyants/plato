using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Users;
using Plato.Internal.Repositories.Roles;

namespace Plato.Internal.Repositories.Users
{

    public class UserRolesRepository : IUserRolesRepository<UserRole>
    {
        #region "Constructor"

        private readonly IDbContext _dbContext;
        private readonly IRoleRepository<Role> _rolesRepository;
        private readonly ILogger<UserSecretRepository> _logger;

        public UserRolesRepository(
            IDbContext dbContext,
            IRoleRepository<Role> rolesRepository,
            ILogger<UserSecretRepository> logger)
        {
            _dbContext = dbContext;
            _rolesRepository = rolesRepository;
            _logger = logger;
        }

        #endregion
        
        #region "Implementation"
        
        public async Task<bool> DeleteAsync(int id)
        {
            bool success;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<bool>(
                    CommandType.StoredProcedure,
                    "DeleteUserRoleById", id);
            }
            return success;
        }

        public async Task<UserRole> InsertUpdateAsync(UserRole userRole)
        {
            var id = 0;
            id = await InsertUpdateInternal(
                userRole.Id,
                userRole.UserId,
                userRole.RoleId);

            if (id > 0)
                return await SelectByIdAsync(id);

            return null;
        }

        public async Task<UserRole> SelectByIdAsync(int id)
        {
            UserRole userRole = null;
            using (var context = _dbContext)
            {
                userRole = await context.ExecuteReaderAsync<UserRole>(
                    CommandType.StoredProcedure,
                    "SelectUserRoleById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            userRole = new UserRole();
                            await reader.ReadAsync();
                            userRole.PopulateModel(reader);
                        }

                        return userRole;
                    },
                    id);

              
            }

            return userRole;
        }

        public async Task<IEnumerable<UserRole>> SelectUserRolesByUserId(int userId)
        {
            IList<UserRole> userRoles = null;
            using (var context = _dbContext)
            {
                userRoles = await context.ExecuteReaderAsync<IList<UserRole>>(
                    CommandType.StoredProcedure,
                    "SelectUserRolesByUserId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            userRoles = new List<UserRole>();
                            while (await reader.ReadAsync())
                            {
                                var userRole = new UserRole();
                                userRole.PopulateModel(reader);
                                userRoles.Add(userRole);
                            }
                     
                        }

                        return userRoles;
                    },
                    userId);
                
            }

            return userRoles;
        }
        
        public async Task<IEnumerable<UserRole>> InsertUserRolesAsync(
            int userId, IEnumerable<string> roleNames)
        {

            List<UserRole> userRoles = null;
            foreach (var roleName in roleNames)
            {
                var role = _rolesRepository.SelectByNameAsync(roleName);
                if (role != null)
                {
                    var userRole = await InsertUpdateAsync(new UserRole()
                    {
                        UserId = userId,
                        RoleId = role.Id
                    });
                    if (userRoles == null)
                        userRoles = new List<UserRole>();
                    userRoles.Add(userRole);
                }
            }

            return userRoles;

        }

        public async Task<IEnumerable<UserRole>> InsertUserRolesAsync(int userId, IEnumerable<int> roleIds)
        {
            List<UserRole> userRoles = null;
            foreach (var roleId in roleIds)
            {
                var role = _rolesRepository.SelectByIdAsync(roleId);
                if (role != null)
                {
                    var userRole = await InsertUpdateAsync(new UserRole()
                    {
                        UserId = userId,
                        RoleId = role.Id
                    });
                    if (userRoles == null)
                        userRoles = new List<UserRole>();
                    userRoles.Add(userRole);
                }
            }

            return userRoles;
        }
        
        public async Task<bool> DeleteUserRolesAsync(int userId)
        {
            bool success;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<bool>(
                    CommandType.StoredProcedure,
                    "DeleteUserRolesByUserId", userId);
            }
            return success;
        }
        
        public async Task<bool> DeleteUserRole(int userId, int roleId)
        {

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteUserRoleByUserIdAndRoleId",
                    userId,
                    roleId);
            }

            return success > 0 ? true : false;

        }
        
        public async Task<IPagedResults<UserRole>> SelectAsync(params object[] inputParams)
        {
            IPagedResults<UserRole> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<UserRole>>(
                    CommandType.StoredProcedure,
                    "SelectUserRolesPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<UserRole>();
                            while (await reader.ReadAsync())
                            {
                                var userRole = new UserRole();
                                userRole.PopulateModel(reader);
                                output.Data.Add(userRole);
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
                    inputParams);

            }

            return output;

        }
        
        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int userId,
            int roleId)
        {
            using (var context = _dbContext)
            {
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUserRole",
                    id,
                    userId,
                    roleId,
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }
        }
        
        #endregion

    }

}