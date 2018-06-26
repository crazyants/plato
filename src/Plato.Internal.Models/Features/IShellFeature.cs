using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Models.Features
{
    public interface IShellFeature
    {

        int Id { get; set; }

        string ModuleId { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        bool IsEnabled { get; set; }

        bool IsRequired { get; set; }
        
        string Version { get; set; }

        IEnumerable<IShellFeature> FeatureDependencies { get; set; }

        IEnumerable<IShellFeature> DependentFeatures { get; set; }

        IEnumerable<IShellFeature> Dependencies { get; set; }

    }

}
