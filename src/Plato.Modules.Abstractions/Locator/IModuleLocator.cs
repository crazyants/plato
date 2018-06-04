using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Modules.Abstractions
{
    public interface IModuleLocator
    {
        Task<IEnumerable<IModuleDescriptor>> LocateModulesAsync(IEnumerable<string> paths, string moduleType, string manifestName, bool manifestIsOptional);

    }
}
