using System.Collections.Generic;

namespace Plato.Internal.Security.Abstractions
{

    public class DefaultPermissions<TPermissions> where TPermissions : class, IPermission
    {

        public string RoleName { get; set; }

        public IEnumerable<TPermissions> Permissions { get; set; }

    }

}
