using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Plato.Environment.Modules.Abstractions;

namespace Plato.Environment.Modules
{
    public class ModuleEntry : IModuleEntry
    {

        public IModuleDescriptor Descriptor
        {
            get; set; 
        }

        public IEnumerable<Assembly> Assmeblies
        {
            get; set; 
        }

        public IEnumerable<Type> ExportedTypes { get; set;  }
        
        public ModuleEntry()
        {
            Assmeblies = new List<Assembly>();        
        }

    }
}
