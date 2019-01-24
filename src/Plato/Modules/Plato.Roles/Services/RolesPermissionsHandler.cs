using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Models.Roles;
using Plato.Internal.Security.Abstractions;

namespace Plato.Roles.Services
{
    public class RolesPermissionsHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly RoleManager<Role> _roleManager;

        public RolesPermissionsHandler(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        #region "Implementation"

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            PermissionRequirement requirement)
        {
            if (context.HasSucceeded)
            {
                // This handler is not revoking any pre-existing grants.
                return;
            }

            // Determine which set of permissions would satisfy the access check
            var grantingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            PermissionNames(requirement.Permission, grantingNames);

            // Determine what set of roles should be examined by the access check
            var rolesToExamine = new List<string> { DefaultRoles.Anonymous };

            if (context.User.Identity.IsAuthenticated)
            {
                rolesToExamine.Add(DefaultRoles.Member);
                // Add roles from the user
                foreach (var claim in context.User.Claims)
                {
                    if (claim.Type == ClaimTypes.Role)
                    {
                        rolesToExamine.Add(claim.Value);
                    }
                }
            }

            foreach (var roleName in rolesToExamine)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    foreach (var claim in role.RoleClaims)
                    {
                        if (!String.Equals(claim.ClaimType, Permission.ClaimTypeName, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        var permissionName = claim.ClaimValue;
                        if (grantingNames.Contains(permissionName))
                        {
                            context.Succeed(requirement);
                            return;
                        }
                    }
                }
            }
        }

        #endregion

        #region "Private Methods"

        static void PermissionNames(
            IPermission permission, 
            HashSet<string> stack)
        {
            // The given name is tested
            stack.Add(permission.Name);

            // Iterate implied permissions to grant, it present
            if (permission.ImpliedBy != null && permission.ImpliedBy.Any())
            {
                foreach (var impliedBy in permission.ImpliedBy)
                {
                    // Avoid potential recursion
                    if (stack.Contains(impliedBy.Name))
                    {
                        continue;
                    }

                    // Otherwise accumulate the implied permission names recursively
                    PermissionNames(impliedBy, stack);
                }
            }

            // Administrator permission grants them all
            //stack.Add(StandardPermissions.Administrator.Name);
        }

        #endregion

    }
}
