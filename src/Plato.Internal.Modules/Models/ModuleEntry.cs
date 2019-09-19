using System;
using System.Collections.Generic;
using System.Reflection;
using Plato.Internal.Models.Modules;

namespace Plato.Internal.Modules.Models
{
    public class ModuleEntry : IModuleEntry
    {
        public ModuleEntry()
        {
            Assemblies = new List<Assembly>();
        }

        public IModuleDescriptor Descriptor { get; set; }

        public IEnumerable<Assembly> Assemblies { get; set; }

        public Assembly Assembly { get; set; }
            
        public IEnumerable<Type> ExportedTypes { get; set; }

    }

}