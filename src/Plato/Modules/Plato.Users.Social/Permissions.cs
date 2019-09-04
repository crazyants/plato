using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Users.Social
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission EditSocial = 
            new Permission("EditSocialUserFields", "Edit social fields");

        public static readonly Permission ViewSocial =
            new Permission("ViewSocialUserFields", "View social fields");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                EditSocial,
                ViewSocial
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
                        EditSocial,
                        ViewSocial
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ViewSocial
                    }
                }
            };

        }

    }

}
