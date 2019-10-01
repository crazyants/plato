using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Issues.History
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ViewEntityHistory =
            new Permission("ViewIssueHistory", "View issue edit history");

        public static readonly Permission viewReplyHistory =
            new Permission("ViewIssueCommentHistory", "View issue comment edit history");
        
        public static readonly Permission RevertEntityHistory =
            new Permission("RevertIssueHistory", "Revert issues to previous versions");

        public static readonly Permission RevertReplyHistory =
            new Permission("RevertIssueCommentHistory", "Revert issue comments to previous versions");
        
        public static readonly Permission DeleteEntityHistory =
            new Permission("DeleteIssueHistory", "Delete issue versions");

        public static readonly Permission DeleteReplyHistory =
            new Permission("DeleteIssueCommentHistory", "Delete issue comment versions");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewEntityHistory,
                viewReplyHistory,
                RevertEntityHistory,
                RevertReplyHistory,
                DeleteEntityHistory,
                DeleteReplyHistory
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
                        ViewEntityHistory,
                        viewReplyHistory,
                        RevertEntityHistory,
                        RevertReplyHistory,
                        DeleteEntityHistory,
                        DeleteReplyHistory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                       ViewEntityHistory,
                        viewReplyHistory,
                        RevertEntityHistory,
                        RevertReplyHistory
                    }
                }
            };
        }

    }

}
