using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Plato.Internal.Security.Abstractions
{
    public static class AuthorizationServiceExtensions
    {

        public static async Task<bool> AuthorizeAsync(
            this IAuthorizationService service,
            ClaimsPrincipal principal,
            IPermission permission)
        {
            var result = await service.AuthorizeAsync(principal, null, new PermissionRequirement(permission));
            return result.Succeeded ? true : false;
        }

        public static async Task<bool> AuthorizeAsync(
            this IAuthorizationService service,
            ClaimsPrincipal principal,
            object resource,
            IPermission permission)
        {
            var result = await service.AuthorizeAsync(principal, resource, new PermissionRequirement(permission));
            return result.Succeeded ? true : false;
        }


    }
}
