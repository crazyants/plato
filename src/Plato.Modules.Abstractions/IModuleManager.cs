using System.Collections.Generic;
using System.Reflection;

namespace Plato.Modules.Abstractions
{
    public interface IModuleManager
    {
        
        IEnumerable<Assembly> AllAvailableAssemblies { get;  }

        IEnumerable<IModuleEntry> AvailableModules { get; }

        void LoadModules();

    }
}
