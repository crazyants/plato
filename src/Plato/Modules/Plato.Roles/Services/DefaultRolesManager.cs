using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Models.Roles;
using Plato.Internal.Security.Abstractions;

namespace Plato.Roles.Services
{
    /// <summary>
    /// Adds the default roles required by Plato.
    /// </summary>
    public class DefaultRolesManager : IDefaultRolesManager
    {

        private readonly IEnumerable<IPermissionsProvider<Permission>> _permissionProviders;
        private readonly RoleManager<Role> _roleManager;

        public DefaultRolesManager(
            IEnumerable<IPermissionsProvider<Permission>> permissionProviders,
            RoleManager<Role> roleManager)
        {
            _permissionProviders = permissionProviders;
            _roleManager = roleManager;
        }

        #region "Implementation"

        public async Task InstallDefaultRolesAsync()
        {

            await InstallInternalAsync(_permissionProviders);

            // Iterate through all permission providers
            foreach (var permissionProvider in _permissionProviders)
            {

                // Get default permissions from provider
                var defaultPermissions = permissionProvider.GetDefaultPermissions();

                // Iterate through default permissions
                // Create a role with the permissions if one does not exist
                // If role exists merge found default permissions with role

                foreach (var defaultPermission in defaultPermissions)
                {
                    var role = await _roleManager.FindByNameAsync(defaultPermission.RoleName);

                    // Create the role
                    if (role == null)
                    {
                        role = new Role { Name = defaultPermission.RoleName };
                        await _roleManager.CreateAsync(role);
                    }

                    // Merge the default permissions into the new or existing role
                    var defaultPermissionNames = (defaultPermission.Permissions ?? Enumerable.Empty<Permission>()).Select(x => x.Name);
                    var currentPermissionNames = ((Role)role).RoleClaims.Where(x => x.ClaimType == Permission.ClaimTypeName).Select(x => x.ClaimValue).ToList();

                    var distinctPermissionNames = currentPermissionNames
                        .Union(defaultPermissionNames)
                        .Distinct();

                    // Update role if set of permissions has increased
                    var additionalPermissionNames = distinctPermissionNames.Except(currentPermissionNames).ToList();

                    if (additionalPermissionNames.Count > 0)
                    {
                        foreach (var permissionName in additionalPermissionNames)
                        {
                            await _roleManager.AddClaimAsync(role, new Claim(Permission.ClaimTypeName, permissionName));
                        }
                    }

                }

            }

        }

        public async Task UpdateDefaultRolesAsync(IPermissionsProvider<Permission> permission)
        {
            await UpdateDefaultRolesAsync(new List<IPermissionsProvider<Permission>>()
            {
                permission
            });
        }

        public async Task UpdateDefaultRolesAsync(IEnumerable<IPermissionsProvider<Permission>> permissions)
        {
            await InstallInternalAsync(_permissionProviders);
        }

        public async Task UninstallDefaultRolesAsync()
        {
            foreach (var roleName in DefaultRoles.ToList())
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    await _roleManager.DeleteAsync(role);
                }
            }
        }

        #endregion

        #region "Private Methods"

        async Task InstallInternalAsync(IEnumerable<IPermissionsProvider<Permission>> permissions)
        {

            // Iterate through all permission providers
            foreach (var permissionProvider in permissions)
            {

                // Get default permissions from provider
                var defaultPermissions = permissionProvider.GetDefaultPermissions();

                // Iterate through default permissions
                // Create a role with the permissions if one does not exist
                // If role exists merge found default permissions with role

                foreach (var defaultPermission in defaultPermissions)
                {
                    var role = await _roleManager.FindByNameAsync(defaultPermission.RoleName);

                    // Create the role
                    if (role == null)
                    {
                        role = new Role { Name = defaultPermission.RoleName };
                        await _roleManager.CreateAsync(role);
                    }

                    // Merge the default permissions into the new or existing role
                    var defaultPermissionNames = (defaultPermission.Permissions ?? Enumerable.Empty<Permission>()).Select(x => x.Name);
                    var currentPermissionNames = ((Role)role).RoleClaims.Where(x => x.ClaimType == Permission.ClaimTypeName).Select(x => x.ClaimValue).ToList();

                    var distinctPermissionNames = currentPermissionNames
                        .Union(defaultPermissionNames)
                        .Distinct();

                    // Update role if set of permissions has increased
                    var additionalPermissionNames = distinctPermissionNames.Except(currentPermissionNames).ToList();

                    if (additionalPermissionNames.Count > 0)
                    {
                        foreach (var permissionName in additionalPermissionNames)
                        {
                            await _roleManager.AddClaimAsync(role, new Claim(Permission.ClaimTypeName, permissionName));
                        }
                    }

                }

            }

        }

        #endregion



    }

}
