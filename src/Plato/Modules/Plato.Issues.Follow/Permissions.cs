using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Issues.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        // FollowIssues
        // AutoFollowIssues
        // AutoFollowIssueComments

        public static readonly Permission FollowIssues =
            new Permission("FollowIssues", "Can follow issues");

        public static readonly Permission AutoFollowIssues =
            new Permission("AutoFollowIssues", "Automatically follow when posting new issues");

        public static readonly Permission AutoFollowIssueComments =
            new Permission("AutoFollowIssueComments", "Automatically follow issues when posting comments");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowIssues,
                AutoFollowIssues,
                AutoFollowIssueComments
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
                        FollowIssues,
                        AutoFollowIssues,
                        AutoFollowIssueComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowIssues,
                        AutoFollowIssues,
                        AutoFollowIssueComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowIssues,
                        AutoFollowIssues,
                        AutoFollowIssueComments
                    }
                }
            };

        }

    }

}
