using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
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
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteRoleById", id);
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
                role.Name.ToEmptyIfNull().TrimToSize(255),
                role.NormalizedName.ToEmptyIfNull().TrimToSize(255),
                role.Description.ToEmptyIfNull().TrimToSize(500),
                claims,
                role.CreatedDate.ToDateIfNull(),
                role.CreatedUserId,
                role.ModifiedDate.ToDateIfNull(),
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
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectRoleById", id);

                if ((reader != null) && (reader.HasRows))
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
                    "SelectRoleByName", name);

                if ((reader != null) && (reader.HasRows))
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
                    "SelectRoleByNameNormalized", normalizedName);

                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    role = new Role();
                    role.PopulateModel(reader);
                }
            }

            return role;
        }

        public async Task<IList<Role>> SelectByUserIdAsync(int userId)
        {
            var output = new List<Role>();
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectRolesByUserId",
                    userId
                );

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
            }

            return output;

        }

        public async Task<IPagedResults<T>> SelectAsync<T>(params object[] inputParams) where T : class
        {
            PagedResults<T> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectRolesPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<T>();
                    while (await reader.ReadAsync())
                    {
                        var role = new Role();
                        role.PopulateModel(reader);
                        output.Data.Add((T) Convert.ChangeType(role, typeof(T)));
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


        public async Task<IPagedResults<Role>> SelectAsync(params object[] inputParams)
        {
            PagedResults<Role> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectRolesPaged",
                    inputParams
                );

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
                        await reader.ReadAsync();
                        output.PopulateTotal(reader);
                    }
                }
            }

            return output;
        }


        public async Task<IEnumerable<Role>> SelectRoles()
        {
            var output = new List<Role>();
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectRoles");

                if ((reader != null) && (reader.HasRows))
                {
                    while (await reader.ReadAsync())
                    {
                        var role = new Role();
                        role.PopulateModel(reader);
                        output.Add(role);
                    }
                }
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
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateRole",
                    id,
                    name.TrimToSize(255),
                    nameNormalized.ToEmptyIfNull().TrimToSize(255),
                    description.ToEmptyIfNull().TrimToSize(255),
                    claims.ToEmptyIfNull(),
                    createdDate.ToDateIfNull(),
                    createdUserId,
                    modifiedDate.ToDateIfNull(),
                    modifiedUserId,
                    concurrencyStamp.ToEmptyIfNull().TrimToSize(255)
                );
            }
        }

        #endregion

    }

}