using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Data.Abstractions.Schemas
{

    public class SchemaDescriptor
    {
        
        public string Version { get; set; }

        public Version TypedVersion { get; set;  }

        public string InstallSql { get; set; }

        public string UpgradeSql { get; set; }

        public string RollbackSql { get; set; }

    }
}
