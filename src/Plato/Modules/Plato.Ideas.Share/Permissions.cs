using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Ideas.Share
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ShareIdeas =
            new Permission("ShareIdeas", "Share ideas");

        public static readonly Permission ShareComments =
            new Permission("ShareIdeaComments", "Share idea comments");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ShareIdeas,
                ShareComments
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
                        ShareIdeas,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ShareIdeas,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ShareIdeas,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ShareIdeas,
                        ShareComments
                    }
                }
            };

        }

    }

}
