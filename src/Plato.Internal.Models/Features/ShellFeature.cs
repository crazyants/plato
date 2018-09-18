using System;
using System.Collections.Generic;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Modules;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Models.Features
{

    public class ShellFeatureSettings : Serializable
    {

        public string DisplayText { get; set; }

    }

    public class ShellFeature : IShellFeature, IModel<ShellFeature>
    {

        public int Id { get; set; }

        public string ModuleId { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsRequired { get; set; }

        public string Version { get; set; }

        public string Settings { get; set; }

        public ShellFeatureSettings FeatureSettings { get; set; } = new ShellFeatureSettings();

        public IEnumerable<IShellFeature> FeatureDependencies { get; set; } = new List<ShellFeature>();

        public IEnumerable<IShellFeature> DependentFeatures { get; set; } = new List<ShellFeature>();
        
        public IEnumerable<IShellFeature> Dependencies { get; set; } = new List<ShellFeature>();

        public ShellFeature()
        {

        }

        public ShellFeature(ShellModule module)
        {
            this.Id = module.Id;
            this.ModuleId = module.ModuleId;
            this.Version = module.Version;
        }

        public ShellFeature(ModuleDependency dependency)
        {
            // dependency.Id to  this.ModuleId is intended
            // dependency.Id is the value deserialized from module.txt
            this.ModuleId = dependency.Id;
            this.Version = dependency.Version;
        }
        
        public ShellFeature(IModuleEntry entry)
        {
            this.ModuleId = entry.Descriptor.Id;
            this.Name = entry.Descriptor.Name;
            this.Description = entry.Descriptor.Description;
            this.Version = entry.Descriptor.Version;

            // Add minimal set of dependencies
            var dependencies = new List<ShellFeature>();
            foreach (var dependency in entry.Descriptor.Dependencies)
            {
                dependencies.Add(new ShellFeature(dependency));
            }

            this.Dependencies = dependencies;
            
        }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("ModuleId"))
                ModuleId = Convert.ToString(dr["ModuleId"]);

            if (dr.ColumnIsNotNull("Version"))
                Version = Convert.ToString(dr["Version"]);

            if (dr.ColumnIsNotNull("Settings"))
                Settings = Convert.ToString(dr["Settings"]);
        }
    }

}
