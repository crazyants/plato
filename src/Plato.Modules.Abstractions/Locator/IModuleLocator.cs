using System.Collections.Generic;

namespace Plato.Environment.Modules.Abstractions
{
    public interface IModuleLocator
    {

        IEnumerable<IModuleDescriptor> LocateModules(IEnumerable<string> paths, string moduleType, string manifestName, bool manifestIsOptional);

    }
}
