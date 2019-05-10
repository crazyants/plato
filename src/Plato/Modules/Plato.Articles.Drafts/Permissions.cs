using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles.Drafts
{
    public class Permissions : IPermissionsProvider<Permission>
    {


        public static readonly Permission ArticlesDraftCreatePrivate =
            new Permission("DraftArticleCreatePrivate", "Create private articles");

        public static readonly Permission ArticlesDraftCreateHidden =
            new Permission("DraftArticleCreateHidden", "Create hidden articles");

        public static readonly Permission ArticlesDraftCreatePublic =
            new Permission("DraftArticleCreatePublic", "Create public articles");

        public static readonly Permission ArticlesDraftToPrivate =
            new Permission("DraftArticleToPrivate", "Convert articles to private");

        public static readonly Permission ArticlesDraftToHidden =
            new Permission("DraftArticleToHidden", "Convert articles to hidden");

        public static readonly Permission ArticlesDraftToPublic =
            new Permission("DraftArticleToPublic", "Convert articles to public");


        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ArticlesDraftCreatePrivate,
                ArticlesDraftCreateHidden,
                ArticlesDraftCreatePublic,
                ArticlesDraftToPrivate,
                ArticlesDraftToHidden,
                ArticlesDraftToPublic
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
                        ArticlesDraftCreatePrivate,
                        ArticlesDraftCreateHidden,
                        ArticlesDraftCreatePublic,
                        ArticlesDraftToPrivate,
                        ArticlesDraftToHidden,
                        ArticlesDraftToPublic
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ArticlesDraftCreatePrivate,
                        ArticlesDraftCreateHidden,
                        ArticlesDraftCreatePublic,
                        ArticlesDraftToPrivate,
                        ArticlesDraftToHidden,
                        ArticlesDraftToPublic
                    }
                }
            };

        }

    }

}
