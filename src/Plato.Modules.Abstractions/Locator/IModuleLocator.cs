using System.Collections.Generic;

namespace Plato.Modules.Abstractions
{
    public interface IModuleLocator
    {
        IEnumerable<IModuleDescriptor> LocateModules(IEnumerable<string> paths, string moduleType, string manifestName, bool manifestIsOptional);

    }
}
