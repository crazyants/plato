using System;
using System.Collections.Generic;
using System.Reflection;
using Plato.Modules.Abstractions;

namespace Plato.Internal.Modules.Models
{
    public class ModuleEntry : IModuleEntry
    {
        public ModuleEntry()
        {
            Assmeblies = new List<Assembly>();
        }

        public IModuleDescriptor Descriptor { get; set; }

        public IEnumerable<Assembly> Assmeblies { get; set; }

        public IEnumerable<Type> ExportedTypes { get; set; }
    }
}