using System.Collections.Generic;
using Plato.Internal.Models.Modules;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Modules.Models
{
    public class ModuleDescriptor : IModuleDescriptor
    {

        public string Id { get; set;  }

        public string Name { get; set;  }

        public string Description { get; set; }

        public string Category { get; set; }

        public string ModuleType { get; set; }

        public string Version { get; set; }

        public string PlatoVersion { get; set; }

        public string Author { get; set; }

        public string WebSite { get; set; }

        public string Tags { get; set; }

        public string Location { get; set;  }

        public string Path { get; set; }

        public string VirtualPathToBin { get; set; }

        public string VirtualPathToModule { get; set; }
        
        public IEnumerable<ModuleDependency> Dependencies { get; set; }

    }
    

}
