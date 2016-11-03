using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Abstractions.Collections;
using Plato.Abstractions.Extensions;
using Plato.Data;
using Plato.Models.Roles;
using Plato.Models.Users;
using Plato.Repositories.Roles;

namespace Plato.Repositories.Users
{
    public class UserRolesRepository : IUserRolesRepository<UserRole>
    {
        #region "Constructor"

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

        #region "Private Variables"

        private readonly IDbContext _dbContext;
        private readonly IRoleRepository<Role> _rolesRepository;
        private readonly ILogger<UserSecretRepository> _logger;

        #endregion

        #region "Implementation"

        public async Task<bool> DeleteAsync(int id)
        {
            bool success;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<bool>(
                    CommandType.StoredProcedure,
                    "plato_sp_DeleteUserRole", id);
            }
            return success;
        }

        public async Task<UserRole> InsertUpdateAsync(UserRole userRole)
        {
            var id = 0;
            id = await InsertUpdateInternal(
                userRole.Id,
                userRole.UserId,
                userRole.RoleId,
                userRole.CreatedDate,
                userRole.CreatedUserId,
                userRole.ModifiedDate,
                userRole.ModifiedUserId,
                userRole.ConcurrencyStamp);

            if (id > 0)
                return await SelectByIdAsync(id);

            return null;
        }

        public async Task<UserRole> SelectByIdAsync(int id)
        {
            UserRole userRole = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectUserRole", id);

                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    userRole = new UserRole();
                    userRole.PopulateModel(reader);
                }
            }

            return userRole;
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
        
        public async Task<bool> DeletetUserRolesAsync(int userId)
        {
            bool success;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<bool>(
                    CommandType.StoredProcedure,
                    "plato_sp_DeleteUserRoles", userId);
            }
            return success;
        }

        public async Task<bool> DeletetUserRole(int userId, string roleName)
        {
            bool success;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<bool>(
                    CommandType.StoredProcedure,
                    "plato_sp_DeleteUserRole", userId);
            }
            return success;
        }

        public Task<bool> DeletetUserRole(int userId, int roleId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int userId,
            int roleId,
            DateTime? createdDate,
            int createdUserId,
            DateTime? modifiedDate,
            int modifiedUserId,
            string concurrencyStamp)
        {
            using (var context = _dbContext)
            {
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "plato_sp_InsertUpdateUserRole",
                    id,
                    userId,
                    roleId,
                    createdDate,
                    createdUserId,
                    modifiedDate,
                    modifiedUserId,
                    concurrencyStamp.ToEmptyIfNull().TrimToSize(50));
            }
        }


        public Task<IPagedResults<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserRole>> SelectUserRoles(int userId)
        {
            List<UserRole> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectUserRoles", 
                    userId
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new List<UserRole>();
                    while (await reader.ReadAsync())
                    {
                        var userRole = new UserRole();
                        userRole.PopulateModel(reader);
                        if (userRole.RoleId > 0)
                        {
                            userRole.Role = new Role();
                            userRole.Role.PopulateModel(reader);
                        }
                        output.Add(userRole);
                    }

                }
            }


            return output;

        }

        #endregion
    }
}