using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Models.Modules;

namespace Plato.Internal.Models.Features
{

    public interface IShellFeature
    {

    }

    public class ShellFeature : IShellFeature
    {

        public string Id { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }

        public bool IsEnabled { get; set; }
        
        public string Version { get; set; }

        public IEnumerable<ShellFeature> FeatureDependencies { get; set; } = new List<ShellFeature>();

        public IEnumerable<ShellFeature> DependentFeatures { get; set; } = new List<ShellFeature>();


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
        }


    }

}
