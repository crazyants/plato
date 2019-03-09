using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles.Reactions
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ReactToArticles =
            new Permission("ReactToArticles", "React to articles");

        public static readonly Permission ReactToComments =
            new Permission("ReactToComments", "React to article comments");

        public static readonly Permission ViewArticleReactions =
            new Permission("ViewArticleReactions", "View article reactions");

        public static readonly Permission ViewCommentReactions =
            new Permission("ViewCommentReactions", "View comment reactions");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ReactToArticles,
                ReactToComments,
                ViewArticleReactions,
                ViewCommentReactions
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
                        ReactToArticles,
                        ReactToComments,
                        ViewArticleReactions,
                        ViewCommentReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ReactToArticles,
                        ReactToComments,
                        ViewArticleReactions,
                        ViewCommentReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ReactToArticles,
                        ReactToComments,
                        ViewArticleReactions,
                        ViewCommentReactions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ViewArticleReactions,
                        ViewCommentReactions
                    }
                }
            };

        }

    }

}
