using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Internal.Models.Users;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Moderation.Models;
using Plato.Moderation.Stores;

namespace Plato.Moderation.Services
{

    public class ModeratorPermissionsHandler : AuthorizationHandler<PermissionRequirement<ModeratorPermission>>
    {

        private readonly IPlatoUserStore<User> _userStore;
        private readonly IModeratorStore<Moderator> _moderatorStore;

        public ModeratorPermissionsHandler(
            IModeratorStore<Moderator> moderatorStore,
            IPlatoUserStore<User> userStore)
        {
            _moderatorStore = moderatorStore;
            _userStore = userStore;
        }

        #region "Implementation"

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement<ModeratorPermission> requirement)
        {

            if (context.HasSucceeded)
            {
                // This handler is not revoking any pre-existing grants.
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

            // The resource represents the channel we are checking against
            if (context.Resource == null)
            {
                return;
            }
            
            // Determine which set of permissions would satisfy the access check
            var grantingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            PermissionNames(requirement.Permission, grantingNames);

            // Get username from claims
            var claims = context.User.Claims
                .FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultNameClaimType);

            // We need a user
            if (claims == null)
            {
                return;
            }

            // Get authenticated user
            var user = await _userStore.GetByUserNameAsync(claims.Value);

            // We need a user to perform access checks against
            if (user == null)
            {
                return;
            }

            // Get all moderator entries for given identity
            var userEntries = moderators.Data
                .Where(m => m.UserId == user.Id)
                .ToList();

            // Reduce our user entries to only parents of current resource we are checking
            
            var moderatorsToExamine = new List<Moderator>();
            foreach (var moderator in userEntries)
            {
                moderatorsToExamine.Add(moderator);
            }

            foreach (var moderator in moderatorsToExamine)
            {
                foreach (var claim in moderator.Claims)
                {
                    if (!String.Equals(claim.ClaimType, ModeratorPermission.ClaimType, StringComparison.OrdinalIgnoreCase))
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
            stack.Add(StandardPermissions.Administrator.Name);
        }

        #endregion



    }

}
