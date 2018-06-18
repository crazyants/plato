using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Features
{
    public class ShellFeatureEventArgs : EventArgs
    {

        public IShellFeature Feature { get; set; }

    }
}
