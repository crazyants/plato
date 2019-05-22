using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Plato.Internal.Data.Migrations
{

    public enum MigrationType
    {
        Install,
        Upgrade,
        Rollback
    }
    
    public interface IDataMigrationBuilder
    {

        MigrationType DataMigrationType { get;  }

        IDataMigrationBuilder BuildMigrations(List<string> versions);

        Task<DataMigrationResult> ApplyMigrationsAsync();

    }
}
