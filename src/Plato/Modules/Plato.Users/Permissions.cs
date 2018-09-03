using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Users
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ManageUsers =
            new Permission("ManageUsers", "Can manage users");
        
        public static readonly Permission AddUsers = 
            new Permission("AddUsers", "Can add new users");

        public static readonly Permission EditUsers =
            new Permission("EditUsers", "Can edit existing users");

        public static readonly Permission DeleteUsers =
            new Permission("DeleteUsers", "Can delete existing users");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageUsers,
                AddUsers,
                EditUsers,
                DeleteUsers
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
                        ManageUsers,
                        AddUsers,
                        EditUsers,
                        DeleteUsers
                    }
                }
            };

        }

    }

}
