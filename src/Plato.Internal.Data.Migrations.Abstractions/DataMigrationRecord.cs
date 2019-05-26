using System.Collections.Generic;

namespace Plato.Internal.Data.Migrations.Abstractions
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
