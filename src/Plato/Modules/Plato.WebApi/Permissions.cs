using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.WebApi
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ManageWebApiSettings =
            new Permission("ManageWebApiSettings", "Manage web api settings");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageWebApiSettings
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
                        ManageWebApiSettings
                    }
                }
            };

        }

    }

}
