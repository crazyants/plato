using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Security.Abstractions
{
    public class DefaultPermissions
    {

        public string Name { get; set; }

        public IEnumerable<Permission> Permissions { get; set; }

    }

}
