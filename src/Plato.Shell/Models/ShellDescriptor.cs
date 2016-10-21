using System.Collections.Generic;

namespace Plato.Shell.Models
{
    public class ShellDescriptor
    {

        public int SerialNumber { get; set;  }

        public IList<ShellModule> Modules { get; set; } = new List<ShellModule>();

    }
}
