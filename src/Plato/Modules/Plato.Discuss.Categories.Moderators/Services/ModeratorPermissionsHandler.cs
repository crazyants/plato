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

namespace Plato.Discuss.Categories.Moderators.Services
{

    //public class ModeratorPermissionsHandler : AuthorizationHandler<PermissionRequirement>
    //{

    //    private readonly IPlatoUserStore<User> _userStore;
    //    private readonly IModeratorStore<Moderator> _moderatorStore;
    //    private readonly IPermissionsManager<ModeratorPermission> _permissionsManager;

    //    public ModeratorPermissionsHandler(
    //        IModeratorStore<Moderator> moderatorStore,
    //        IPlatoUserStore<User> userStore,
    //        IPermissionsManager<ModeratorPermission> permissionsManager)
    //    {
    //        _moderatorStore = moderatorStore;
    //        _userStore = userStore;
    //        _permissionsManager = permissionsManager;
    //    }

    //    #region "Implementation"

    //    protected override async Task HandleRequirementAsync(
    //        AuthorizationHandlerContext context,
    //        PermissionRequirement requirement)
    //    {

    //        if (context.HasSucceeded)
    //        {
    //            // This handler is not revoking any pre-existing grants.
    //            return;
    //        }

    //        // We always need to be authenticated for moderator permissions to apply
    //        if (!context.User.Identity.IsAuthenticated)
    //        {
    //            return;
    //        }

    //        // The resource represents the category we are checking against
    //        if (context.Resource == null)
    //        {
    //            return;
    //        }

    //        // Ensure we can convert our resource Id
    //        var validChannel = int.TryParse(context.Resource.ToString(), out var categoryId);
    //        if (!validChannel)
    //        {
    //            return;
    //        }

    //        // Get all moderators
    //        var moderators = await _moderatorStore
    //            .QueryAsync()
    //            .ToList();

    //        // No need to check permissions if we don't have any moderators
    //        if (moderators == null)
    //        {
    //            return;
    //        }

    //        // Get username from claims
    //        var claims = context.User.Claims
    //            .FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultNameClaimType);

    //        // We need a user
    //        if (claims == null)
    //        {
    //            return;
    //        }

    //        // Get authenticated user
    //        var user = await _userStore.GetByUserNameAsync(claims.Value);

    //        // We need a user to perform access checks against
    //        if (user == null)
    //        {
    //            return;
    //        }

    //        // Get all moderator entries for given identity and resource
    //        var userEntries = moderators.Data
    //            .Where(m => m.UserId == user.Id & m.CategoryId == categoryId)
    //            .ToList();

    //        // No moderator entries for the user and resource
    //        if (!userEntries.Any())
    //        {
    //            return;
    //        }

    //        // Reduce our user entries to only parents of current resource we are checking

    //        var moderatorsToExamine = new List<Moderator>();
    //        foreach (var moderator in userEntries)
    //        {
    //            moderatorsToExamine.Add(moderator);
    //        }

    //        // Determine which set of permissions would satisfy the access check
    //        var grantingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    //        var permissions = _permissionsManager.GetPermissions();

    //        ImpliedPermissionNames(requirement.Permission, grantingNames);
    //        InferredPermissionNames(requirement.Permission, permissions.ToList(), grantingNames);

    //        foreach (var moderator in moderatorsToExamine)
    //        {
    //            foreach (var claim in moderator.Claims)
    //            {
    //                if (!String.Equals(claim.ClaimType, ModeratorPermission.ClaimTypeName,
    //                    StringComparison.OrdinalIgnoreCase))
    //                {
    //                    continue;
    //                }

    //                var permissionName = claim.ClaimValue;
    //                if (grantingNames.Contains(permissionName))
    //                {
    //                    context.Succeed(requirement);
    //                    return;
    //                }
    //            }
    //        }

    //    }

    //    #endregion

    //    #region "Private Methods"

    //    static void InferredPermissionNames(
    //        IPermission permission, // EditAnyTopic
    //        IList<ModeratorPermission> permissions,
    //        HashSet<string> stack)
    //    {

    //        // The given name is always tested
    //        stack.Add(permission.Name);

    //        // Iterate available permissions looking for inferred permissions for the given permission
    //        foreach (var permissionToCheck in permissions)
    //        {
    //            if (permission.ImpliedBy != null && permission.ImpliedBy.Any())
    //            {
    //                foreach (var impliedBy in permission.ImpliedBy)
    //                {
    //                    if (impliedBy.Name.Equals(permission.Name, StringComparison.CurrentCultureIgnoreCase))
    //                    {
    //                        // Avoid potential recursion
    //                        if (stack.Contains(permission.Name))
    //                        {
    //                            continue;
    //                        }

    //                        // Otherwise accumulate the inferred permission names recursively
    //                        InferredPermissionNames(impliedBy, permissions, stack);
    //                    }
    //                }
    //            }
    //        }

    //    }
        
    //    static void ImpliedPermissionNames(
    //        IPermission permission,
    //        HashSet<string> stack)
    //    {
    //        // The given name is tested
    //        stack.Add(permission.Name);

    //        // Iterate implied permissions to grant, it present
    //        if (permission.ImpliedBy != null && permission.ImpliedBy.Any())
    //        {
    //            foreach (var impliedBy in permission.ImpliedBy)
    //            {
    //                // Avoid potential recursion
    //                if (stack.Contains(impliedBy.Name))
    //                {
    //                    continue;
    //                }

    //                // Otherwise accumulate the implied permission names recursively
    //                ImpliedPermissionNames(impliedBy, stack);
    //            }
    //        }

    //        // Administrator permission grants them all
    //        //stack.Add(StandardPermissions.Administrator.Name);
    //    }

    //    #endregion
        
    //}

}
