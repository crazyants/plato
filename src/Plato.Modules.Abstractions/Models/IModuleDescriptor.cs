using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Modules.Abstractions
{
    public interface IModuleDescriptor
    {
        string ID { get; set; }

        string ModuleType { get; set; }

        string Name { get; set; }

        string Location { get; set; }
           
        string VirtualPathToBin { get; set; }
    }
}
