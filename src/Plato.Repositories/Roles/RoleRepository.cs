using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

            var dbId = 0;
            using (var context = _dbContext)
            {
                dbId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "plato_sp_InsertUpdateRole",
                    id,
                    permissionId,
                    name,
                    description.ToEmptyIfNull(),
                    htmlPrefix.ToEmptyIfNull(),
                    htmlSuffix.ToEmptyIfNull(),
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
                    concurrencyStamp.ToEmptyIfNull()
                );
            }

            return dbId;
        }

        #endregion

        #region "Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<RoleRepository> _logger;

        #endregion

        #region "Implementation"

        public Task<bool> DeleteAsync(int id)
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

        public Task<Role> SelectByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Role> SelectByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Role>> SelectPagedAsync(int pageIndex, int pageSize, object options)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}