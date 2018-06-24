using Microsoft.AspNetCore.Authorization;
using System;

namespace Plato.Internal.Security.Abstractions
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(Permission permission)
        {
            Permission = permission ??
                         throw new ArgumentNullException(nameof(permission));
        }

        public Permission Permission { get; set; }
    }

}
