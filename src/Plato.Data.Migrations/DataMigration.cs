using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data.Migrations
{
    public class DataMigration
    {

        public IEnumerable<string> Statements { get; set; }

        public string Version { get; set; }

    }
}
