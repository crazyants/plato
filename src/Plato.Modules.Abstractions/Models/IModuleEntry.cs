using System;
using System.Collections.Generic;
using System.Reflection;

namespace Plato.Environment.Modules.Abstractions
{
    public interface IModuleEntry
    {

        IModuleDescriptor Descriptor { get; set; }

        IEnumerable<Assembly> Assmeblies { get; set; }

        IEnumerable<Type> ExportedTypes { get; set; }

    }
}
