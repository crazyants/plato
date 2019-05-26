using System.Threading.Tasks;

namespace Plato.Internal.Data.Migrations.Abstractions
{

    public interface IDataMigrationManager
    {
        Task<DataMigrationResult> ApplyMigrationsAsync(DataMigrationRecord dataMigrationRecord);
    }

}
