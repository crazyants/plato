using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Search
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ManageSearchSettings =
            new Permission("ManageSearchSettings", "Manage search settings");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageSearchSettings
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
                        ManageSearchSettings
                    }
                }
            };

        }

    }

}
