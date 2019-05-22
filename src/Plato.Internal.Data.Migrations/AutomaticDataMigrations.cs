using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Migrations
{

    public class AutomaticDataMigrations
    {

        private readonly IDataMigrationBuilder _migrationBuilder;
        private readonly IDataMigrationManager _migrationManager;

        #region "Constructor"

        public AutomaticDataMigrations(
            IDataMigrationBuilder migrationBuilder,
            IDataMigrationManager migrationManager)
        {
            _migrationBuilder = migrationBuilder;
            _migrationManager = migrationManager;
        }

        #endregion

        #region "Private Methods"

        public async Task<DataMigrationResult> InitialMigration()
        {
            return await _migrationBuilder.BuildMigrations(
                new List<string>()
                {
                    "1.0.0"
                }).ApplyMigrationsAsync();
        }
        
        #endregion
        
    }

}
