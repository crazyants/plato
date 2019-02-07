using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Share
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ShareTopics =
            new Permission("ShareTopics", "Share topics");

        public static readonly Permission ShareReplies =
            new Permission("ShareReplies", "Share Replies");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ShareTopics,
                ShareReplies
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
                        ShareTopics,
                        ShareReplies
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ShareTopics,
                        ShareReplies
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ShareTopics,
                        ShareReplies
                    }
                }
            };

        }

    }

}
