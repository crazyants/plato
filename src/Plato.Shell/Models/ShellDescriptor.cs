using System.Collections.Generic;

namespace Plato.Shell.Models
{
    public class ShellDescriptor
    {
        
        public IList<ShellModule> Modules { get; set; } = new List<ShellModule>();

    }
}
