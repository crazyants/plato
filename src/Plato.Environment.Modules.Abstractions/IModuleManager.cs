using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Environment.Modules.Abstractions
{
    public interface IModuleManager
    {

        IEnumerable<IModuleEntry> ModuleEntries { get; }

        void LoadModuleDescriptors();

        void LoadModules();

    }
}
