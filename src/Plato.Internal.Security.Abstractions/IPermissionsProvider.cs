using System.Collections.Generic;

namespace Plato.Internal.Security.Abstractions
{

    public interface IPermissionsProvider
    {
        IEnumerable<Permission> GetPermissions();

        IEnumerable<DefaultPermissions> GetDefaultPermissions();

    }

}
