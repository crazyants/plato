using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Reactions
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ReactToTopics =
            new Permission("ReactToTopics", "React To topics");

        public static readonly Permission ReactToReplies =
            new Permission("ReactToReplies", "React To topic replies");

        public static readonly Permission ViewTopicReactions =
            new Permission("ViewTopicReactions", "View topic reactions");

        public static readonly Permission ViewReplyReactions =
            new Permission("ViewReplyReactions", "View topic reply reactions");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ReactToTopics,
                ReactToReplies,
                ViewTopicReactions,
                ViewReplyReactions
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
                        ReactToTopics,
                        ReactToReplies,
                        ViewTopicReactions,
                        ViewReplyReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ReactToTopics,
                        ReactToReplies,
                        ViewTopicReactions,
                        ViewReplyReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ReactToTopics,
                        ReactToReplies,
                        ViewTopicReactions,
                        ViewReplyReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ViewTopicReactions,
                        ViewReplyReactions
                    }
                }
            };

        }

    }

}
