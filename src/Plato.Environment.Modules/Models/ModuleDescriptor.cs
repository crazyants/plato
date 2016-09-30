using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Environment.Modules
{
    public class ModuleDescriptor
    {

        public string ID { get; set;  }

        public string ModuleType { get; set;  }

        public string Name { get; set;  }

        public string Path { get; set;  }

        public string Location { get; set;  }

        public string BinLocation { get; set; }
    }
}
