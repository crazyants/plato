using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Environment.Modules
{
    public interface IModuleLocatorAccessor
    {
        IEnumerable<ModuleDescriptor> LocateModules();
    }
}
