using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Private
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission DiscussPrivateToPublic =
            new Permission("DiscussPrivateAllowPublic", "Enable \"Public\" topics");

        public static readonly Permission DiscussPrivateToPrivate =
            new Permission("DiscussPrivateAllowPrivate", "Enable \"Private\" topics");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                DiscussPrivateToPublic,
                DiscussPrivateToPrivate
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
                        DiscussPrivateToPublic,
                        DiscussPrivateToPrivate
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        DiscussPrivateToPublic,
                        DiscussPrivateToPrivate
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        DiscussPrivateToPublic,
                        DiscussPrivateToPrivate
                    }
                }
            };

        }

    }

}
