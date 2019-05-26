using System.Collections.Generic;

namespace Plato.Internal.Data.Migrations.Abstractions
{
    public class DataMigration
    {

        public IEnumerable<string> Statements { get; set; }

        public string Version { get; set; }

    }
}
