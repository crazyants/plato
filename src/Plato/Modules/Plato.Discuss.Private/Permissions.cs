using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Private
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission DiscussPrivateCreatePublic =
            new Permission("DiscussPrivateAllowPublic", "Post public topics");

        public static readonly Permission DiscussPrivateCreatePrivate =
            new Permission("DiscussPrivateAllowPrivate", "Post private topics");

        public static readonly Permission DiscussPrivateToPublic =
            new Permission("DiscussPrivateToPublic", "Convert topics to public");

        public static readonly Permission DiscussPrivateToPrivate =
            new Permission("DiscussPrivateToPrivate", "Convert topics to private");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                DiscussPrivateCreatePublic,
                DiscussPrivateCreatePrivate,
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
                        DiscussPrivateCreatePublic,
                        DiscussPrivateCreatePrivate,
                        DiscussPrivateToPublic,
                        DiscussPrivateToPrivate
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        DiscussPrivateCreatePublic,
                        DiscussPrivateCreatePrivate,
                        DiscussPrivateToPublic,
                        DiscussPrivateToPrivate
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        DiscussPrivateCreatePublic,
                        DiscussPrivateCreatePrivate,
                        DiscussPrivateToPublic,
                        DiscussPrivateToPrivate
                    }
                }
            };

        }

    }

}
