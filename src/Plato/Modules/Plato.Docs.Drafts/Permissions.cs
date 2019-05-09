using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs.Drafts
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission DraftArticleToPrivate =
            new Permission("DraftArticleToPrivate", "Enable \"Private\"");

        public static readonly Permission DraftArticleToHidden =
            new Permission("DraftArticleToHidden", "Enable \"Ready for Review\"");

        public static readonly Permission DraftArticleToPublic =
            new Permission("DraftArticleToPublic", "Enable \"Public / Publish\"");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
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
                        DraftArticleToPrivate,
                        DraftArticleToHidden,
                        DraftArticleToPublic
                    }
                }
            };

        }

    }

}
