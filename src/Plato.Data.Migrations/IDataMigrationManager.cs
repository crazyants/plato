using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Data.Migrations
{
    public interface IDataMigrationManager
    {

        DataMigrationResult ApplyMigrations(DataMigrationRecord dataMigrationRecord);

    }

}
