using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Ideas.Private
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission IdeasPrivateCreatePublic =
            new Permission("IdeasPrivateCreatePublic", "Post public ideas");

        public static readonly Permission IdeasPrivateCreatePrivate =
            new Permission("IdeasPrivateCreatePrivate", "Post private ideas");

        public static readonly Permission IdeasPrivateToPublic =
            new Permission("IdeasPrivateToPublic", "Convert ideas to public");

        public static readonly Permission IdeasPrivateToPrivate =
            new Permission("IdeasPrivateToPrivate", "Convert ideas to private");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                IdeasPrivateCreatePublic,
                IdeasPrivateCreatePrivate,
                IdeasPrivateToPublic,
                IdeasPrivateToPrivate
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
                        IdeasPrivateCreatePublic,
                        IdeasPrivateCreatePrivate,
                        IdeasPrivateToPublic,
                        IdeasPrivateToPrivate
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        IdeasPrivateCreatePublic,
                        IdeasPrivateCreatePrivate,
                        IdeasPrivateToPublic,
                        IdeasPrivateToPrivate
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        IdeasPrivateCreatePublic,
                        IdeasPrivateCreatePrivate,
                        IdeasPrivateToPublic,
                        IdeasPrivateToPrivate
                    }
                }
            };

        }

    }

}
