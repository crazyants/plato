using System.Collections.Generic;

namespace Plato.Internal.Data.Migrations
{
    public class DataMigration
    {

        public IEnumerable<string> Statements { get; set; }

        public string Version { get; set; }

    }
}
