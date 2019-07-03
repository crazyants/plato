using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Moderation.Models;
using Plato.Moderation.Stores;

namespace Plato.Moderation.Services
{

    public class ModeratorPermissionsHandler : AuthorizationHandler<PermissionRequirement>
    {
        
        private readonly IPermissionsManager<ModeratorPermission> _permissionsManager;
        private readonly IModeratorStore<Moderator> _moderatorStore;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly IContextFacade _contextFacade;

        public ModeratorPermissionsHandler(
            IPermissionsManager<ModeratorPermission> permissionsManager,
            IModeratorStore<Moderator> moderatorStore,
            IPlatoUserStore<User> userStore,
            IContextFacade contextFacade)
        {
            _permissionsManager = permissionsManager;
            _moderatorStore = moderatorStore;
            _contextFacade = contextFacade;
            _userStore = userStore;
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

            // We always need to be authenticated for moderator permissions to apply
            if (!context.User.Identity.IsAuthenticated)
            {
                return;
            }

            // The resource represents the category we are checking against
            if (context.Resource == null)
            {
                return;
            }

            // Ensure we can convert our resource Id
            var validChannel = int.TryParse(context.Resource.ToString(), out var categoryId);
            if (!validChannel)
            {
                return;
            }

            // Get all moderators
            var moderators = await _moderatorStore
                .QueryAsync()
                .ToList();

            // No need to check permissions if we don't have any moderators
            if (moderators == null)
            {
                return;
            }

            // Get supplied user
            var user = await _contextFacade.GetAuthenticatedUserAsync(context.User.Identity);

            // We need a user to perform access checks against
            if (user == null)
            {
                return;
            }

            // Get all moderator entries for given user and resource
            var userEntries = moderators.Data
                .Where(m => m.UserId == user.Id & m.CategoryId == categoryId)
                .ToList();

            // No moderator entries for the user and resource
            if (!userEntries.Any())
            {
                return;
            }

            // Reduce our user entries to only parents of current resource we are checking

            var moderatorsToExamine = new List<Moderator>();
            foreach (var moderator in userEntries)
            {
                moderatorsToExamine.Add(moderator);
            }

            // Accumulate the set of permissions that would satisfy the access check
            var grantingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
            // Accumulate permissions implied by supplied permission
            ImpliedPermissionNames(requirement.Permission, grantingNames);

            // Accumulate permissions that imply supplied permission
            var permissions = _permissionsManager.GetPermissions();
            if (permissions != null)
            {
                InferredPermissionNames(requirement.Permission, permissions.ToList(), grantingNames);
            }
            
            // Determine if  we satisfy any of the accumulated permissions
            foreach (var moderator in moderatorsToExamine)
            {
                foreach (var claim in moderator.Claims)
                {
                    if (!String.Equals(claim.ClaimType, ModeratorPermission.ClaimTypeName,
                        StringComparison.OrdinalIgnoreCase))
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

        #endregion

        #region "Private Methods"

        void InferredPermissionNames(
            IPermission permissionToCheck, 
            IList<ModeratorPermission> permissions,
            HashSet<string> stack)
        {

            // The given name is always tested
            stack.Add(permissionToCheck.Name);

            // Iterate available permissions looking for inferred permissions (those that imply the given permission)
            foreach (var permission in permissions)
            {
                if (permission.ImpliedBy != null && permission.ImpliedBy.Any())
                {
                    foreach (var impliedBy in permission.ImpliedBy)
                    {
                        // Is our supplied permission implied by the permission
                        if (impliedBy.Name.Equals(permissionToCheck.Name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            // The permission that inferred the given permission was already added
                            if (stack.Contains(permission.Name))
                            {
                                continue;
                            }

                            // Otherwise accumulate the inferred permission names recursively
                            InferredPermissionNames(permission, permissions, stack);
                        }
                    }
                }
            }

        }

        void ImpliedPermissionNames(
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
                    ImpliedPermissionNames(impliedBy, stack);
                }
            }

        }

        #endregion

    }
    
}
