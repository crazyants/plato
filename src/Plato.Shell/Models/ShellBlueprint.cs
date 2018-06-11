using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Shell.Models
{
    public class ShellBlueprint
    {
        public ShellSettings Settings { get; set; }

        public ShellDescriptor Descriptor { get; set; }

        public IDictionary<Type, IModuleEntry> Dependencies { get; set; }

    }
}
