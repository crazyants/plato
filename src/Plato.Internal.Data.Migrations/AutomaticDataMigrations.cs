using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Data.Migrations.Abstractions;

namespace Plato.Internal.Data.Migrations
{

    public class AutomaticDataMigrations
    {

        private readonly IDataMigrationBuilder _migrationBuilder;
    

        #region "Constructor"

        public AutomaticDataMigrations(
            IDataMigrationBuilder migrationBuilder)
        {
            _migrationBuilder = migrationBuilder;
        }

        #endregion

        #region "Private Methods"

        public async Task<DataMigrationResult> InitialMigrationAsync()
        {

            var migrations = _migrationBuilder.BuildMigrations(
                new List<string>()
                {
                    "1.0.0",
                    "1.0.1"
                });

            return await migrations.ApplyMigrationsAsync();

        }
        
        #endregion
        
    }

}
