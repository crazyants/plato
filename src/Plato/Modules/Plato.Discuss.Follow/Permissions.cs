using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        // FollowTopics
        // AutoFollowTopics
        // AutoFollowTopicReplies

        public static readonly Permission FollowTopics =
            new Permission("FollowTopics", "Can follow topics");

        public static readonly Permission AutoFollowTopics =
            new Permission("AutoFollowTopics", "Automatically follow when posting new topics");

        public static readonly Permission AutoFollowTopicReplies =
            new Permission("AutoFollowTopicReplies", "Automatically follow topics when posting replies");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowTopics,
                AutoFollowTopics,
                AutoFollowTopicReplies
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
                        AutoFollowTopics,
                        AutoFollowTopicReplies
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowTopics,
                        AutoFollowTopics,
                        AutoFollowTopicReplies
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowTopics,
                        AutoFollowTopics,
                        AutoFollowTopicReplies
                    }
                }
            };

        }

    }

}
