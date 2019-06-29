using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Facebook
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission EditFacebookSettings =
            new Permission("EditFacebookSettings", "Manage facebook settings");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                EditFacebookSettings
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
                        EditFacebookSettings
                    }
                }
            };

        }

    }

}
