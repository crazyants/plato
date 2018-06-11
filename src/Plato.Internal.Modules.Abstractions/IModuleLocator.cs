using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Modules.Abstractions
{
    public interface IModuleLocator
    {
        Task<IEnumerable<IModuleDescriptor>> LocateModulesAsync(IEnumerable<string> paths, string moduleType, string manifestName, bool manifestIsOptional);

    }
}
