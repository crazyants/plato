using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Plato.Internal.Security.Abstractions
{
    public static class AuthorizationServiceExtensions
    {

        public static Task<bool> AuthorizeAsync<TPermission>(
            this IAuthorizationService service,
            ClaimsPrincipal principal,
            IPermission permission) where TPermission : class, IPermission 
            => AuthorizeAsync<TPermission>(service, principal, null, permission);
        
        public static async Task<bool> AuthorizeAsync<TPermission>(
            this IAuthorizationService service,
            ClaimsPrincipal principal,
            object resource,
            IPermission permission) where TPermission : class, IPermission
        {
            var result = await service.AuthorizeAsync(
                principal, 
                resource, 
                new PermissionRequirement<TPermission>(permission));
            return result.Succeeded ? true : false;
        }
        
    }

}
