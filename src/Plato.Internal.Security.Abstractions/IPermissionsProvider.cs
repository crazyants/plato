using System.Collections.Generic;

namespace Plato.Internal.Security.Abstractions
{
    
    public interface IPermissionsProvider<TPermissions> where TPermissions : class, IPermission
    {
        IEnumerable<TPermissions> GetPermissions();

        IEnumerable<DefaultPermissions<TPermissions>> GetDefaultPermissions();

    }

}
