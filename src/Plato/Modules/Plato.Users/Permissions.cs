using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Users
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ViewUsers =
            new Permission("ViewUsers", "Can view users");

        public static readonly Permission ViewProfiles =
            new Permission("ViewProfiles", "Can view user profiles");

        public static readonly Permission ManageUsers =
            new Permission("ManageUsers", "Can manage users");
        
        public static readonly Permission AddUsers = 
            new Permission("AddUsers", "Can add new users");

        public static readonly Permission EditUsers =
            new Permission("EditUsers", "Can edit existing users");

        public static readonly Permission UserToVerify =
            new Permission("UserToVerify", "Can mark users as verified");

        public static readonly Permission UserToStaff =
            new Permission("UserToStaff", "Can mark users as staff");
        
        public static readonly Permission UserToSpam =
            new Permission("UserToSpam", "Can mark users as SPAM");

        public static readonly Permission UserToBanned =
            new Permission("UserToBanned", "Can mark users as banned");

        public static readonly Permission ResetUserPasswords =
            new Permission("ResetUserPasswords", "Can reset user passwords");

        public static readonly Permission DeleteUsers =
            new Permission("DeleteUsers", "Can delete existing users");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewUsers,
                ViewProfiles,
                ManageUsers,
                AddUsers,
                EditUsers,
                UserToVerify,
                UserToStaff,
                UserToSpam,
                UserToBanned,
                ResetUserPasswords,
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
                        ViewProfiles,
                        ManageUsers,
                        AddUsers,
                        EditUsers,
                        UserToVerify,
                        UserToStaff,
                        UserToSpam,
                        UserToBanned,
                        ResetUserPasswords,
                        DeleteUsers
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ViewUsers,
                        ViewProfiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ViewUsers,
                        ViewProfiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ViewUsers,
                        ViewProfiles
                    }
                }
            };

        }

    }

}
