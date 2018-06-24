using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Users.Social
{
    public class PermissionsProvider : IPermissionsProvider
    {

        public static readonly Permission EditSocial = 
            new Permission("EditSocialUserFields", "Can edit social user fields?");

        public static readonly Permission ViewSocial =
            new Permission("ViewSocialUserFields", "Can view social user fields?");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                EditSocial,
                ViewSocial
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
                        EditSocial,
                        ViewSocial
                    }
                },
                new DefaultPermissions
                {
                    Name = "Members",
                    Permissions = new[]
                    {
                        ViewSocial
                    }
                }
            };

        }

    }

}
