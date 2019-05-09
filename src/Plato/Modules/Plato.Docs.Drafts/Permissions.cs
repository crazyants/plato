using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs.Drafts
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission DraftDocToPrivate =
            new Permission("DraftDocToPrivate", "Enable \"Private\"");

        public static readonly Permission DraftDocToHidden =
            new Permission("DraftDocToHidden", "Enable \"Ready for Review\"");

        public static readonly Permission DraftDocToPublic =
            new Permission("DraftDocToPublic", "Enable \"Public / Publish\"");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                DraftDocToPrivate,
                DraftDocToHidden,
                DraftDocToPublic
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
                        DraftDocToPrivate,
                        DraftDocToHidden,
                        DraftDocToPublic
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        DraftDocToPrivate,
                        DraftDocToHidden,
                        DraftDocToPublic
                    }
                }
            };

        }

    }

}
