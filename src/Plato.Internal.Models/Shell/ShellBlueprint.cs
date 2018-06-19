using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Models.Modules;

namespace Plato.Internal.Models.Shell
{
    public class ShellBlueprint
    {
        public IShellSettings Settings { get; set; }

        public IShellDescriptor Descriptor { get; set; }

        public IDictionary<Type, IModuleEntry> Dependencies { get; set; }

    }
}
