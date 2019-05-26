using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Features.Updates
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ManageFeatureUpdates =
            new Permission("ManageFeatureUpdates", "Can view available feature updates");
        
        public static readonly Permission UpdateFeatures = 
            new Permission("UpdateFeatures", "Can install available feature updates");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageFeatureUpdates,
                UpdateFeatures
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
                        ManageFeatureUpdates,
                        UpdateFeatures
                    }
                }
            };

        }

    }

}
