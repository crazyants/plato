using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles.Reactions
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ReactToTopics =
            new Permission("ReactToTopics", "React To Topics");

        public static readonly Permission ReactToReplies =
            new Permission("ReactToReplies", "React To Replies");

        public static readonly Permission ViewReactions =
            new Permission("ViewReactions", "View Reactions");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ReactToTopics,
                ReactToReplies,
                ViewReactions
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
                        ViewReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ReactToTopics,
                        ReactToReplies,
                        ViewReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ReactToTopics,
                        ReactToReplies,
                        ViewReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ViewReactions
                    }
                }
            };

        }

    }

}
