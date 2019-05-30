using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Data.Migrations.Abstractions;

namespace Plato.Internal.Data.Migrations
{

    public class AutomaticDataMigrations
    {

        public const string InitialVersion = "0.0.0";

        private readonly IOptions<PlatoOptions> _platoOptions;
        private readonly IDataMigrationBuilder _migrationBuilder;
    

        #region "Constructor"

        public AutomaticDataMigrations(
            IDataMigrationBuilder migrationBuilder,
            IOptions<PlatoOptions> platoOptions)
        {
            _migrationBuilder = migrationBuilder;
            _platoOptions = platoOptions;
        }

        #endregion

        #region "Private Methods"

        public async Task<DataMigrationResult> InitialMigrationAsync()
        {
            
            // Start and end versions
            var from = InitialVersion.ToVersion();
            var to = _platoOptions.Value.Version.ToVersion();

            // Validate versions

            if (from == null)
            {
                throw new ArgumentOutOfRangeException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentOutOfRangeException(nameof(to));
            }
            
            // All versions between from and to
            var versions = from.GetVersionsBetween(to);

            // Compile versions to search
            var versionsToSearch = versions?.Select(v => v.ToString());

            // Ensure we have versions to search
            if (versionsToSearch == null)
            {
                return null;
            }

            // Get all schemas 
            var migrations = _migrationBuilder
                .BuildMigrations(versionsToSearch.ToArray());

            return await migrations.ApplyMigrationsAsync();
            

        }
        
        #endregion
        
    }

}
