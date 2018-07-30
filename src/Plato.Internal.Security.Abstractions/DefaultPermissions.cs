using System.Collections.Generic;

namespace Plato.Internal.Security.Abstractions
{
    public class DefaultPermissions
    {

        public string RoleName { get; set; }

        public IEnumerable<Permission> Permissions { get; set; }

    }

}
