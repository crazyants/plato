using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Environment.Modules
{
    public interface IModuleLocator
    {
        IEnumerable<ModuleDescriptor> LocateModuless(IEnumerable<string> paths, string extensionType, string manifestName, bool manifestIsOptional);
    }
}
