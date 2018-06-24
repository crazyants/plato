using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Roles
{
    public class PermissionsProvider : IPermissionsProvider
    {

        public static readonly Permission ManageRoles =
            new Permission("ManageRoles", "Can manage roles");
        
        public static readonly Permission AddRoles = 
            new Permission("AddRoles", "Can add new roles");

        public static readonly Permission EditRoles =
            new Permission("EditRoles", "Can edit existing roles");

        public static readonly Permission DeleteRoles =
            new Permission("DeleteRoles", "Can delete existing roles");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageRoles,
                AddRoles,
                EditRoles,
                DeleteRoles
            };
        }

        public IEnumerable<DefaultPermissions> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissions
                {
                    Name = "Administrator",
                    Permissions = new[]
                    {
                        ManageRoles,
                        AddRoles,
                        EditRoles,
                        DeleteRoles
                    }
                }
            };

        }

    }

}
