using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Environment.Shell.Models
{
    public class ShellDescriptor
    {

        public int SerialNumber { get; set;  }

        public IList<ShellFeature> Features { get; set; } = new List<ShellFeature>();

    }
}
