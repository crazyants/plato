using System;
using System.Collections.Generic;
using System.Reflection;

namespace Plato.Internal.Models.Modules
{
    public interface IModuleEntry
    {

        IModuleDescriptor Descriptor { get; set; }

        IEnumerable<Assembly> Assmeblies { get; set; }

        Assembly Assembly { get; set; }
        
        IEnumerable<Type> ExportedTypes { get; set; }

    }
}
