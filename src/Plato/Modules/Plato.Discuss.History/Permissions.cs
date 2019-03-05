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

        public static readonly Permission DeleteTopicHistory =
            new Permission("DeleteTopicHistory", "Delete topic edit history");

        public static readonly Permission DeleteReplyHistory =
            new Permission("DeleteReplyHistory", "Delete reply edit history");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewTopicHistory,
                ViewReplyHistory,
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
