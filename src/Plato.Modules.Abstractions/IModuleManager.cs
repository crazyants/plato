using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Environment.Modules.Abstractions.Features;
using System.Reflection;

namespace Plato.Environment.Modules.Abstractions
{
    public interface IModuleManager
    {


        IEnumerable<Assembly> AvailableAssemblies { get;  }

        IEnumerable<IModuleEntry> ModuleEntries { get; }

        void LoadModuleDescriptors();

        void LoadModules();

    }
}
