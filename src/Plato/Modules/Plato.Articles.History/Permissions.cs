using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles.History
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ViewEntityHistory =
            new Permission("ViewArticleHistory", "View article edit history");

        public static readonly Permission viewReplyHistory =
            new Permission("ViewArticleCommentHistory", "View article comment edit history");
        
        public static readonly Permission RevertEntityHistory =
            new Permission("RevertArticleHistory", "Revert articles to previous versions");

        public static readonly Permission RevertReplyHistory =
            new Permission("RevertArticleCommentHistory", "Revert article comments to previous versions");
        
        public static readonly Permission DeleteEntityHistory =
            new Permission("DeleteArticleHistory", "Delete article versions");

        public static readonly Permission DeleteReplyHistory =
            new Permission("DeleteArticleCommentHistory", "Delete article comment versions");
        
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
