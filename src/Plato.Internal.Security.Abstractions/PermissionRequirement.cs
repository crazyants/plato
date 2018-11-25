using Microsoft.AspNetCore.Authorization;
using System;

namespace Plato.Internal.Security.Abstractions
{
    public class PermissionRequirement<TPermission> : IAuthorizationRequirement where TPermission : class, IPermission
    {
        public PermissionRequirement(IPermission permission)
        {
            Permission = permission ??
                         throw new ArgumentNullException(nameof(permission));
        }

        public IPermission Permission { get; set; }
    }

}
