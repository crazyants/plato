using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Features.ViewModels
{
    public class FEaturesViewModel
    {
        public IEnumerable<IModuleEntry> Modules { get; set; }

    }
}
