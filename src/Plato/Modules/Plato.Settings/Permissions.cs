using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Settings
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ManageGeneralSettings =
            new Permission("ManageGeneralSettings", "Manage general settings");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageGeneralSettings
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
                        ManageGeneralSettings
                    }
                }
            };

        }

    }

}
