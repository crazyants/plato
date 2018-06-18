using System.Collections.Generic;
using Plato.Internal.Models.Modules;

namespace Plato.Internal.Models.Features
{
  

    public class ShellFeature : IShellFeature
    {

        public string Id { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }

        public bool IsEnabled { get; set; }
        
        public string Version { get; set; }

        public IEnumerable<IShellFeature> FeatureDependencies { get; set; } = new List<ShellFeature>();

        public IEnumerable<IShellFeature> DependentFeatures { get; set; } = new List<ShellFeature>();
        
        public IEnumerable<IShellFeature> Dependencies { get; set; } = new List<ShellFeature>();

        public ShellFeature()
        {

        }

        public ShellFeature(string id)
        {
            this.Id = id;
        }

        public ShellFeature(string id, string version)
        {
            this.Id = id;
            this.Version = version;
        }
        
        public ShellFeature(IModuleEntry entry)
        {
            this.Id = entry.Descriptor.Id;
            this.Name = entry.Descriptor.Name;
            this.Description = entry.Descriptor.Description;
            this.Version = entry.Descriptor.Version;

            // Add minimal set of dependencies
            var dependencies = new List<ShellFeature>();
            foreach (var dependency in entry.Descriptor.Dependencies)
            {
                dependencies.Add(new ShellFeature(dependency.Id, dependency.Version));
            }

            this.Dependencies = dependencies;
            
        }


    }

}
