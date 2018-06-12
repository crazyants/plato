using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Internal.Modules.Abstractions
{
    public interface IModuleDescriptor
    {
        string Id { get; set; }

        string Name { get; set; }

        string Description { get; set; }
        
        string Category { get; set; }

        string ModuleType { get; set; }
        
        string Version { get; set; }

        string PlatoVersion { get; set; }

        string Author { get; set; }

        string WebSite { get; set; }

        string Tags { get; set; }


        string Location { get; set; }
           
        string Path { get; set; }

        string VirtualPathToBin { get; set; }

        IEnumerable<ModuleDependency> Dependencies { get; set; }
        
    }



    public class ModuleDependency
    {

        public string Name { get; set; }

        public string Version { get; set; }

    }



}
