using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs.Reactions
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ReactToDocs =
            new Permission("ReactToDocs", "React to docs");

        public static readonly Permission ReactToDocComments =
            new Permission("ReactToDocComments", "React to doc comments");

        public static readonly Permission ViewDocReactions =
            new Permission("ViewDocReactions", "View doc reactions");

        public static readonly Permission ViewDocCommentReactions =
            new Permission("ViewDocCommentReactions", "View doc comment reactions");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ReactToDocs,
                ReactToDocComments,
                ViewDocReactions,
                ViewDocCommentReactions
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
                        ReactToDocs,
                        ReactToDocComments,
                        ViewDocReactions,
                        ViewDocCommentReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ReactToDocs,
                        ReactToDocComments,
                        ViewDocReactions,
                        ViewDocCommentReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ReactToDocs,
                        ReactToDocComments,
                        ViewDocReactions,
                        ViewDocCommentReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ViewDocReactions,
                        ViewDocCommentReactions
                    }
                }
            };

        }

    }

}
