using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.History
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ViewTopicHistory =
            new Permission("ViewTopicHistory", "View topic edit history");

        public static readonly Permission ViewReplyHistory =
            new Permission("ViewReplyHistory", "View reply edit history");
        
        public static readonly Permission RevertTopicHistory =
            new Permission("RevertTopicHistory", "Revert topics to previous versions");

        public static readonly Permission RevertReplyHistory =
            new Permission("RevertReplyHistory", "Revert replies to previous versions");
        
        public static readonly Permission DeleteTopicHistory =
            new Permission("DeleteTopicHistory", "Delete topic versions");

        public static readonly Permission DeleteReplyHistory =
            new Permission("DeleteReplyHistory", "Delete reply versions");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewTopicHistory,
                ViewReplyHistory,
                RevertTopicHistory,
                RevertReplyHistory,
                DeleteTopicHistory,
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
                        ViewTopicHistory,
                        ViewReplyHistory,
                        RevertTopicHistory,
                        RevertReplyHistory,
                        DeleteTopicHistory,
                        DeleteReplyHistory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ViewTopicHistory,
                        ViewReplyHistory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ViewTopicHistory,
                        ViewReplyHistory,
                        RevertTopicHistory,
                        RevertReplyHistory,
                        DeleteTopicHistory,
                        DeleteReplyHistory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ViewTopicHistory,
                        ViewReplyHistory
                    }
                }
            };
        }

    }

}
