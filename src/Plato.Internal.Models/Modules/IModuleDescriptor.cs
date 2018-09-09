using System.Collections.Generic;

namespace Plato.Internal.Models.Modules
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

        string VirtualPathToModule { get; set; }

        IEnumerable<ModuleDependency> Dependencies { get; set; }
        
    }
    

    public class ModuleDependency
    {

        public string Id { get; set; }

        public string Version { get; set; }

        public bool IsEnabled { get; set; }

    }

}
