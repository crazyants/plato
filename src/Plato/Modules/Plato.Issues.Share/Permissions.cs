using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Issues.Share
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ShareIssues =
            new Permission("ShareIssues", "Share issues");

        public static readonly Permission ShareComments =
            new Permission("ShareIssueComments", "Share issue comments");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ShareIssues,
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
                        ShareIssues,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ShareIssues,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ShareIssues,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ShareIssues,
                        ShareComments
                    }
                }
            };

        }

    }

}
