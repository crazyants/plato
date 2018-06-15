using System;
using System.Collections.Generic;
using System.Text;

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
        
        public IEnumerable<ShellFeature> FeatureDependencies { get; set; } = new List<ShellFeature>();

        public IEnumerable<ShellFeature> DependentFeatures { get; set; } = new List<ShellFeature>();
        
    }

}
