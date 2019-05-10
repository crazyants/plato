using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Issues.Private
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission IssuesPrivateCreatePublic =
            new Permission("IssuesPrivateCreatePublic", "Post public issues");

        public static readonly Permission IssuesPrivateCreatePrivate =
            new Permission("IssuesPrivateCreatePrivate", "Post private issues");

        public static readonly Permission IssuesPrivateToPublic =
            new Permission("IssuesPrivateToPublic", "Convert issues to public");

        public static readonly Permission IssuesPrivateToPrivate =
            new Permission("IssuesPrivateToPrivate", "Convert issues to private");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                IssuesPrivateCreatePublic,
                IssuesPrivateCreatePrivate,
                IssuesPrivateToPublic,
                IssuesPrivateToPrivate
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
                        IssuesPrivateCreatePublic,
                        IssuesPrivateCreatePrivate,
                        IssuesPrivateToPublic,
                        IssuesPrivateToPrivate
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        IssuesPrivateCreatePublic,
                        IssuesPrivateCreatePrivate,
                        IssuesPrivateToPublic,
                        IssuesPrivateToPrivate
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        IssuesPrivateCreatePublic,
                        IssuesPrivateCreatePrivate,
                        IssuesPrivateToPublic,
                        IssuesPrivateToPrivate
                    }
                }
            };

        }

    }

}
