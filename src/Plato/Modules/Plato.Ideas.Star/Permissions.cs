using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Ideas.Star
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission StarIdeas =
            new Permission("StarIdeas", "Star ideas");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                StarIdeas
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
                        StarIdeas
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        StarIdeas
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        StarIdeas
                    }
                }
            };

        }

    }

}
