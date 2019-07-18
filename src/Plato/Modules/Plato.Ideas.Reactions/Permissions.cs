using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Ideas.Reactions
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ReactToIdeas =
            new Permission("ReactToIdeas", "React to ideas");

        public static readonly Permission ReactToIdeaComments =
            new Permission("ReactToIdeaComments", "React to idea comments");

        public static readonly Permission ViewIdeaReactions =
            new Permission("ViewIdeaReactions", "View idea reactions");

        public static readonly Permission ViewIdeaCommentReactions =
            new Permission("ViewIdeaCommentReactions", "View idea comment reactions");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ReactToIdeas,
                ReactToIdeaComments,
                ViewIdeaReactions,
                ViewIdeaCommentReactions
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
                        ReactToIdeas,
                        ReactToIdeaComments,
                        ViewIdeaReactions,
                        ViewIdeaCommentReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ReactToIdeas,
                        ReactToIdeaComments,
                        ViewIdeaReactions,
                        ViewIdeaCommentReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ReactToIdeas,
                        ReactToIdeaComments,
                        ViewIdeaReactions,
                        ViewIdeaCommentReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ViewIdeaReactions,
                        ViewIdeaCommentReactions
                    }
                }
            };

        }

    }

}
