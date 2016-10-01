using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data.Migrations
{
    public class DataMigration
    {

        public string Upgrade { get; set; }

        public string RollBack { get; set; }

        public int? Version { get; set; }

    }
}
