using System.Collections.Generic;
using Plato.Internal.Models.Features;

namespace Plato.Features.ViewModels
{
    public class FeaturesViewModel
    {
        public IEnumerable<ShellFeature> Features { get; set; }

        public FeaturesBulkAction BulkAction { get; set; }
    }

    public enum FeaturesBulkAction
    {
        None,
        Enable,
        Disable,
        Update,
        Toggle
    }
}
