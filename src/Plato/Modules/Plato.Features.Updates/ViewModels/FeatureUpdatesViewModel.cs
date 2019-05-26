using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plato.Internal.Models.Features;

namespace Plato.Features.Updates.ViewModels
{
    
    public class FeatureUpdatesIndexViewModel
    {

        public FeatureIndexOptions Options { get; set; } = new FeatureIndexOptions();
        
        public IEnumerable<IShellFeature> Features { get; set; }

    }

    public class FeatureIndexOptions
    {
        
        public bool HideEnabled { get; set; }

    }
    
}
