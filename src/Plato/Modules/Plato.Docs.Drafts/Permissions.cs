using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs.Drafts
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission DocsDraftCreatePrivate =
            new Permission("DocsDraftCreatePrivate", "Create private docs");

        public static readonly Permission DocsDraftCreateHidden =
            new Permission("DocsDraftCreateHidden", "Create hidden docs");

        public static readonly Permission DocsDraftCreatePublic =
            new Permission("DocsDraftCreatePublic", "Create public docs");

        public static readonly Permission DocsDraftToPrivate =
            new Permission("DocsDraftToPrivate", "Convert docs to private");

        public static readonly Permission DocsDraftToHidden =
            new Permission("DocsDraftToHidden", "Convert docs to hidden");

        public static readonly Permission DocsDraftToPublic =
            new Permission("DocsDraftToPublic", "Convert docs to public");


        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                DocsDraftCreatePrivate,
                DocsDraftCreateHidden,
                DocsDraftCreatePublic,
                DocsDraftToPrivate,
                DocsDraftToHidden,
                DocsDraftToPublic
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
                        DocsDraftCreatePrivate,
                        DocsDraftCreateHidden,
                        DocsDraftCreatePublic,
                        DocsDraftToPrivate,
                        DocsDraftToHidden,
                        DocsDraftToPublic
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        DocsDraftCreatePrivate,
                        DocsDraftCreateHidden,
                        DocsDraftCreatePublic,
                        DocsDraftToPrivate,
                        DocsDraftToHidden,
                        DocsDraftToPublic
                    }
                }
            };

        }

    }

}
