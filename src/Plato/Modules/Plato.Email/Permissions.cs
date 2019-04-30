using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Email
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ManageEmailSettings =
            new Permission("ManageEmailSettings", "Manage email settings");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageEmailSettings
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
                        ManageEmailSettings
                    }
                }
            };

        }

    }

}
