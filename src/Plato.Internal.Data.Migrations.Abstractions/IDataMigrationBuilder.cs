using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Plato.Internal.Data.Migrations.Abstractions
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
        
        IDataMigrationBuilder BuildMigrations(IList<string> versions);

        IDataMigrationBuilder BuildMigrations(string moduleId, Version from, Version to);
        
        Task<DataMigrationResult> ApplyMigrationsAsync();

    }
}
