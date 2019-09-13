using System;
using System.Collections.Generic;
using System.Reflection;

namespace Plato.Internal.Models.Modules
{
    public interface IModuleEntry
    {

        IModuleDescriptor Descriptor { get; set; }

        IEnumerable<Assembly> Assemblies { get; set; }

        Assembly Assembly { get; set; }

        Assembly ViewsAssembly { get; set; }

        IEnumerable<Type> ExportedTypes { get; set; }

    }
}
