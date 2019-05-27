using System.Collections.Generic;
using Plato.Internal.Models.Features;

namespace Plato.Features.Updates.ViewModels
{
    
    public class FeatureUpdatesViewModel
    {

        public FeatureUpdateOptions Options { get; set; } = new FeatureUpdateOptions();
        
        public IEnumerable<IShellFeature> Features { get; set; }

    }

    public class FeatureUpdateOptions
    {
        
        public bool HideEnabled { get; set; }

    }
    
}
