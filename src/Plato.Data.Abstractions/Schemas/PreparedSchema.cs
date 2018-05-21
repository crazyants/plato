using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Plato.Data.Abstractions.Schemas
{
    public class PreparedSchema
    {
        public string Version { get; set; }

        public Version TypedVersion { get; set; }

        public List<string> InstallStatements { get; set; }

        public List<string> UpgradeStatements { get; set; }

        public List<string> RollbackStatements { get; set; }

    }
}
