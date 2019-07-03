using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission FollowTopics =
            new Permission("FollowTopics", "Can follow topics");

        public static readonly Permission FollowTopicsByDefault =
            new Permission("FollowTopicsByDefault", "Follow posted topics by default");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowTopics,
                FollowTopicsByDefault
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
                        FollowTopics,
                        FollowTopicsByDefault
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowTopics,
                        FollowTopicsByDefault
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowTopics,
                        FollowTopicsByDefault
                    }
                }
            };

        }

    }

}
