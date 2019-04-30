using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Features
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ManageFeatures =
            new Permission("ManageFeatures", "Can manage features");
        
        public static readonly Permission EnableFeatures = 
            new Permission("EnableFeatures", "Can enable features");
        
        public static readonly Permission DisableFeatures =
            new Permission("DisableFeatures", "Can disable features");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageFeatures,
                EnableFeatures,
                DisableFeatures
            };
        }

        public IEnumerable<DefaultPermissions<Permission>> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        ManageFeatures,
                        EnableFeatures,
                        DisableFeatures
                    }
                }
            };

        }

    }

}
