using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Plato.Modules.Abstractions
{
    public interface IModuleManager
    {
        
        Task<IEnumerable<Assembly>> LoadModuleAssembliesAsync();

        Task<IEnumerable<IModuleEntry>> LoadModulesAsync();

        Task<IEnumerable<IModuleEntry>> LoadModulesAsync(string[] moduleIds);

    }
}
