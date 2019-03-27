using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Votes
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission VoteTopics =
            new Permission("VoteTopics", "Can up & down vote topics");

        public static readonly Permission VoteTopicReplies =
            new Permission("VoteTopicReplies", "Can up & down vote topic replies");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                VoteTopics,
                VoteTopicReplies
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
                        VoteTopics,
                        VoteTopicReplies
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        VoteTopics,
                        VoteTopicReplies
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        VoteTopics,
                        VoteTopicReplies
                    }
                }
            };
        }

    }

}
