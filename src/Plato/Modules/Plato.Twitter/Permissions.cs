using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Twitter
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission EditTwitterSettings =
            new Permission("EditTwitterSettings", "Manage twitter settings");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                EditTwitterSettings
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
                        EditTwitterSettings
                    }
                }
            };

        }

    }

}
