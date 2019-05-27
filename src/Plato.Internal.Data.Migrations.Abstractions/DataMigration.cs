using System.Collections.Generic;

namespace Plato.Internal.Data.Migrations.Abstractions
{
    public class DataMigration
    {

        public string ModuleId { get; set; }

        public string Version { get; set; }

        public IEnumerable<string> Statements { get; set; }

    }
}
