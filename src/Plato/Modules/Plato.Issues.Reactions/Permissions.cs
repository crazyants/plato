using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Issues.Reactions
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ReactToIssues =
            new Permission("ReactToIssues", "React to issues");

        public static readonly Permission ReactToIssueComments =
            new Permission("ReactToIssueComments", "React to issue comments");

        public static readonly Permission ViewIssueReactions =
            new Permission("ViewIssueReactions", "View issue reactions");

        public static readonly Permission ViewIssueCommentReactions =
            new Permission("ViewIssueCommentReactions", "View issue comment reactions");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ReactToIssues,
                ReactToIssueComments,
                ViewIssueReactions,
                ViewIssueCommentReactions
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
                        ReactToIssues,
                        ReactToIssueComments,
                        ViewIssueReactions,
                        ViewIssueCommentReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ReactToIssues,
                        ReactToIssueComments,
                        ViewIssueReactions,
                        ViewIssueCommentReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ReactToIssues,
                        ReactToIssueComments,
                        ViewIssueReactions,
                        ViewIssueCommentReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ViewIssueReactions,
                        ViewIssueCommentReactions
                    }
                }
            };

        }

    }

}
