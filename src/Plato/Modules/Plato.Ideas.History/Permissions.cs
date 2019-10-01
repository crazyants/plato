using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Ideas.History
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ViewEntityHistory =
            new Permission("ViewIdeaHistory", "View idea edit history");

        public static readonly Permission viewReplyHistory =
            new Permission("ViewIdeaCommentHistory", "View idea comment edit history");
        
        public static readonly Permission RevertEntityHistory =
            new Permission("RevertDocHistory", "Revert ideas to previous versions");

        public static readonly Permission RevertReplyHistory =
            new Permission("RevertIdeaCommentHistory", "Revert idea comments to previous versions");
        
        public static readonly Permission DeleteEntityHistory =
            new Permission("DeleteIdeaHistory", "Delete idea versions");

        public static readonly Permission DeleteReplyHistory =
            new Permission("DeleteIdeaCommentHistory", "Delete idea comment versions");
        
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
