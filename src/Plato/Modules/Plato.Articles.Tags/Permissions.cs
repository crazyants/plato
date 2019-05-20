using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles.Tags
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostArticleTags =
            new Permission("PostArticleTags", "Can add tags when posting articles");

        public static readonly Permission EditArticleTags =
            new Permission("EditArticleTags", "Can edit tags when editing articles");

        public static readonly Permission PostArticleCommentTags =
            new Permission("PostArticleCommentTags", "Can add tags when posting article comments");

        public static readonly Permission EditArticleCommentTags =
            new Permission("EditArticleCommentTags", "Can edit tags when editing article comments");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostArticleTags,
                EditArticleTags,
                PostArticleCommentTags,
                EditArticleCommentTags
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
                        PostArticleTags,
                        EditArticleTags,
                        PostArticleCommentTags,
                        EditArticleCommentTags
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostArticleTags,
                        EditArticleTags,
                        PostArticleCommentTags,
                        EditArticleCommentTags
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostArticleTags,
                        EditArticleTags,
                        PostArticleCommentTags,
                        EditArticleCommentTags
                    }
                }
            };

        }

    }

}
