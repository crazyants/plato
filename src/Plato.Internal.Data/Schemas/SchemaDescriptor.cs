using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Data.Schemas
{

    public class SchemaDescriptor
    {
        
        public string Version { get; set; }

        public string InstallSql { get; set; }

        public string UpgradeSql { get; set; }

        public string RollbackSql { get; set; }

    }
}
