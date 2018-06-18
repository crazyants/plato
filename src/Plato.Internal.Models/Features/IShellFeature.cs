using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Models.Features
{
    public interface IShellFeature
    {

        string Id { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        bool IsEnabled { get; set; }

        string Version { get; set; }

        IEnumerable<IShellFeature> FeatureDependencies { get; set; }

        IEnumerable<IShellFeature> DependentFeatures { get; set; }

        IEnumerable<IShellFeature> Dependencies { get; set; }

    }

}
