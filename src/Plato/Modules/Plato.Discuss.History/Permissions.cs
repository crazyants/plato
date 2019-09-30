using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.History
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ViewEntityHistory =
            new Permission("ViewTopicHistory", "View topic edit history");

        public static readonly Permission viewReplyHistory =
            new Permission("ViewReplyHistory", "View reply edit history");
        
        public static readonly Permission RevertEntityHistory =
            new Permission("RevertTopicHistory", "Revert topics to previous versions");

        public static readonly Permission RevertReplyHistory =
            new Permission("RevertReplyHistory", "Revert replies to previous versions");
        
        public static readonly Permission DeleteEntityHistory =
            new Permission("DeleteTopicHistory", "Delete topic versions");

        public static readonly Permission DeleteReplyHistory =
            new Permission("DeleteReplyHistory", "Delete reply versions");
        
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
