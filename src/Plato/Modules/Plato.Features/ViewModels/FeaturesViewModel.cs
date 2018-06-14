using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Models.Modules;

namespace Plato.Features.ViewModels
{
    public class FEaturesViewModel
    {
        public IEnumerable<IModuleEntry> Modules { get; set; }

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
