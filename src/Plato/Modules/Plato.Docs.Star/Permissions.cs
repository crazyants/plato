using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs.Star
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission StarDocs =
            new Permission("StarDocs", "Star docs");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                StarDocs
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
                        StarDocs
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        StarDocs
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        StarDocs
                    }
                }
            };

        }

    }

}
