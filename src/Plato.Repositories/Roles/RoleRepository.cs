using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Abstractions.Collections;
using Plato.Abstractions.Extensions;
using Plato.Data;
using Plato.Models.Roles;

namespace Plato.Repositories.Roles
{
    public class RoleRepository : IRoleRepository<Role>
    {
        #region "Constructor"

        public RoleRepository(
            IDbContext dbContext,
            ILogger<RoleRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int permissionId,
            string name,
            string nameNormalized,
            string description,
            string htmlPrefix,
            string htmlSuffix,
            bool isAdministrator,
            bool isEmployee,
            bool isAnonymous,
            bool isMember,
            bool isWaitingConfirmation,
            bool isBanned,
            int sortOrder,
            DateTime? createdDate,
            int createdUserId,
            DateTime? modifiedDate,
            int modifiedUserId,
            bool isDeleted,
            DateTime? deletedDate,
            int deletedUserId,
            string concurrencyStamp)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            using (var context = _dbContext)
            {
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "plato_sp_InsertUpdateRole",
                    id,
                    permissionId,
                    name.TrimToSize(255),
                    nameNormalized.ToEmptyIfNull().TrimToSize(255),
                    description.ToEmptyIfNull().TrimToSize(255),
                    htmlPrefix.ToEmptyIfNull().TrimToSize(100),
                    htmlSuffix.ToEmptyIfNull().TrimToSize(100),
                    isAdministrator,
                    isEmployee,
                    isAnonymous,
                    isMember,
                    isWaitingConfirmation,
                    isBanned,
                    sortOrder,
                    createdDate,
                    createdUserId,
                    modifiedDate,
                    modifiedUserId,
                    isDeleted,
                    deletedDate,
                    deletedUserId,
                    concurrencyStamp.ToEmptyIfNull().TrimToSize(255)
                );
            }
        }

        #endregion

        #region "Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<RoleRepository> _logger;

        #endregion

        #region "Implementation"

        public Task<Role> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Role> InsertUpdateAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var id = await InsertUpdateInternal(
                role.Id,
                role.PermissionId,
                role.Name,
                role.NormalizedName,
                role.Description,
                role.HtmlPrefix,
                role.HtmlSuffix,
                role.IsAdministrator,
                role.IsEmployee,
                role.IsAnonymous,
                role.IsMember,
                role.IsWaitingConfirmation,
                role.IsBanned,
                role.SortOrder,
                role.CreatedDate,
                role.CreatedUserId,
                role.ModifiedDate,
                role.ModifiedUserId,
                role.IsDeleted,
                role.DeletedDate,
                role.DeletedUserId,
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
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectRole", id);

                if (reader != null)
                {
                    await reader.ReadAsync();
                    role = new Role();
                    role.PopulateModel(reader);
                }
            }

            return role;
        }

        public async Task<Role> SelectByNameAsync(string name)
        {
            Role role = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectRoleByName", name);

                if (reader != null)
                {
                    await reader.ReadAsync();
                    role = new Role();
                    role.PopulateModel(reader);
                }
            }

            return role;
        }

        public async Task<Role> SelectByNormalizedNameAsync(string normalizedName)
        {
            Role role = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectRoleByNameNormalized", normalizedName);

                if (reader != null)
                {
                    await reader.ReadAsync();
                    role = new Role();
                    role.PopulateModel(reader);
                }
            }

            return role;
        }


        public async Task<IPagedResults<T>> SelectAsync<T>(params object[] inputParams) where T : class
        {
            PagedResults<T> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "plato_sp_SelectRolesPaged",
                    inputParams
                );

                if (reader != null)
                {
                    output = new PagedResults<T>();
                    while (await reader.ReadAsync())
                    {
                        var role = new Role();
                        role.PopulateModel(reader);
                        output.Add((T)Convert.ChangeType(role, typeof(T)));
                    }

                    if (await reader.NextResultAsync())
                    {
                        await reader.ReadAsync();
                        output.PopulateTotal(reader);
                    }
                }
            }

            return output;
        }

        #endregion
    }
}