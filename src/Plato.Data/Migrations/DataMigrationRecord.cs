using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data.Migrations
{
    public class DataMigrationRecord
    {

        public DataMigrationRecord()
        {
            Migrations = new List<DataMigration>();
        }

        public List<DataMigration> Migrations { get; set; }

    }

}
