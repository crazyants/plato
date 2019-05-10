using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles.Drafts
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission DraftArticleCreatePrivate =
            new Permission("DraftArticleCreatePrivate", "Create private articles");

        public static readonly Permission DraftArticleCreateHidden =
            new Permission("DraftArticleCreateHidden", "Create hidden articles");

        public static readonly Permission DraftArticleCreatePublic =
            new Permission("DraftArticleCreatePublic", "Create public articles");

        public static readonly Permission DraftArticleToPrivate =
            new Permission("DraftArticleToPrivate", "Convert articles to private");

        public static readonly Permission DraftArticleToHidden =
            new Permission("DraftArticleToHidden", "Convert articles to hidden");

        public static readonly Permission DraftArticleToPublic =
            new Permission("DraftArticleToPublic", "Convert articles to public");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                DraftArticleCreatePrivate,
                DraftArticleCreateHidden,
                DraftArticleCreatePublic,
                DraftArticleToPrivate,
                DraftArticleToHidden,
                DraftArticleToPublic
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
                        DraftArticleCreatePrivate,
                        DraftArticleCreateHidden,
                        DraftArticleCreatePublic,
                        DraftArticleToPrivate,
                        DraftArticleToHidden,
                        DraftArticleToPublic
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        DraftArticleCreatePrivate,
                        DraftArticleCreateHidden,
                        DraftArticleCreatePublic,
                        DraftArticleToPrivate,
                        DraftArticleToHidden,
                        DraftArticleToPublic
                    }
                }
            };

        }

    }

}
