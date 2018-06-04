using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Plato.Modules.Abstractions
{
    public interface IModuleManager
    {
        
        IEnumerable<Assembly> AllAvailableAssemblies { get;  }

        IEnumerable<IModuleEntry> AvailableModules { get; }

        Task LoadModulesAsync();

        Task<IEnumerable<IModuleEntry>> LoadModulesAsync(string[] moduleIds);

    }
}
