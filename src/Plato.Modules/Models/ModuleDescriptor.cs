using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Modules.Abstractions;

namespace Plato.Modules.Models
{
    public class ModuleDescriptor : IModuleDescriptor
    {

        public string Id { get; set;  }

        public string ModuleType { get; set;  }

        public string Name { get; set;  }

        public string Location { get; set;  }

        public string VirtualPathToBin { get; set; }
    }
}
