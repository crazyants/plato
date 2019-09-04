using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Theming
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ManageThemes =
            new Permission("ManageThemes", "Manage themes");

        public static readonly Permission CreateThemes =
            new Permission("CreateThemes", "Create themes");

        public static readonly Permission EditThemes =
            new Permission("EditThemes", "Edit themes");

        public static readonly Permission DeleteThemes =
            new Permission("DeleteThemes", "Delete themes");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageThemes,
                CreateThemes,
                EditThemes,
                DeleteThemes
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
                        ManageThemes,
                        CreateThemes,
                        EditThemes,
                        DeleteThemes
                    }
                }
            };

        }

    }

}
