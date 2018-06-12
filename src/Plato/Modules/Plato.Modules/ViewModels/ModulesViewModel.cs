using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Modules.ViewModels
{
    public class ModulesViewModel
    {
        public IEnumerable<IModuleEntry> Modules { get; set; }

    }
}
