using System.Threading.Tasks;

namespace Plato.Internal.Data.Migrations
{
    public interface IDataMigrationManager
    {

        Task<DataMigrationResult> ApplyMigrationsAsync(DataMigrationRecord dataMigrationRecord);

    }

}
