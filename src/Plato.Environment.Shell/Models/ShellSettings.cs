using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Environment.Shell
{
    public class ShellSettings
    {

        private readonly IDictionary<string, string> _values;
        
        public ShellSettings()
        {
            _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);  
        }


    }
}
