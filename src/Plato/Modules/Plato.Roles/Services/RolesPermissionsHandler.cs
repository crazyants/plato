using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Internal.Models.Roles;

namespace Plato.Roles.Services
{
    public class RolesPermissionsHandler : AuthorizationHandler<PermissionRequirement>
    {

        private readonly IContextFacade _contextFacade;
        private readonly IPlatoRoleStore _platoRoleStore;
        
        public RolesPermissionsHandler(
            IContextFacade contextFacade, 
            IPlatoRoleStore platoRoleStore)
        {
            _contextFacade = contextFacade;
            _platoRoleStore = platoRoleStore;
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
            var rolesToExamine = new List<Role>();

            // Removed for performance reasons but left in place for easy fallback if needed
            //// Search specific roles within supplied identity
            //var roleClaims = context.User
            //    .Claims
            //    .Where(c => c.Type == ClaimTypes.Role)
            //    .ToList();

            //if (roleClaims.Count > 0)
            //{
            //    rolesToExamine.AddRange(roleClaims.Select(r => r.Value));
            //}
            //else
            //{
                // Get authenticated user
                var user = await _contextFacade.GetAuthenticatedUserAsync(context.User.Identity);
                if (user != null)
                {
                    rolesToExamine.AddRange(await _platoRoleStore.GetRolesByUserIdAsync(user.Id));
                }
                else
                {
                rolesToExamine.Add(await _platoRoleStore.GetByNameAsync(DefaultRoles.Anonymous));
                }

            //}
            
            // Iterate roles checking claims
            foreach (var role in rolesToExamine)
            {
                //Role role = await _platoRoleStore.GetByNameAsync(roleName);
                //if (role != null)
                //{
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
                //}
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
