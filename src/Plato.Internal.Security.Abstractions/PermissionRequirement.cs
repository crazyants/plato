using System;
using Microsoft.AspNetCore.Authorization;

namespace Plato.Internal.Security.Abstractions
{
    public class PermissionRequirement : IAuthorizationRequirement 
    {
        public PermissionRequirement(IPermission permission)
        {
            Permission = permission ??
                         throw new ArgumentNullException(nameof(permission));
        }

        public IPermission Permission { get; set; }

    }

}
