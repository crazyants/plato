using System.Collections.Generic;

namespace Plato.Internal.Security.Abstractions
{

    public interface IPermissionsProvider
    {
        IEnumerable<Permission> GetPermissions();

        IEnumerable<DefaultPermissions> GetDefaultPermissions();

    }

    public interface IPermissionsProvider2<TPermissions> where TPermissions : class, IPermission
    {
        IEnumerable<TPermissions> GetPermissions();

        IEnumerable<DefaultPermissions2<TPermissions>> GetDefaultPermissions();

    }


}
