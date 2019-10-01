using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs.History
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ViewEntityHistory =
            new Permission("ViewDocHistory", "View doc edit history");

        public static readonly Permission viewReplyHistory =
            new Permission("ViewDocCommentHistory", "View doc comment edit history");
        
        public static readonly Permission RevertEntityHistory =
            new Permission("RevertDocHistory", "Revert docs to previous versions");

        public static readonly Permission RevertReplyHistory =
            new Permission("RevertDocCommentHistory", "Revert doc comments to previous versions");
        
        public static readonly Permission DeleteEntityHistory =
            new Permission("DeleteDocHistory", "Delete doc versions");

        public static readonly Permission DeleteReplyHistory =
            new Permission("DeleteDocCommentHistory", "Delete doc comment versions");
        
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
