using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Users
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ViewUsers =
            new Permission("ViewUsers", "Can view users");

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
                ViewUsers,
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
                        ViewUsers,
                        ManageUsers,
                        AddUsers,
                        EditUsers,
                        DeleteUsers
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ViewUsers
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ViewUsers
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ViewUsers
                    }
                }
            };

        }

    }

}
