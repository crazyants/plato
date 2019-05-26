using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Data.Migrations.Abstractions;

namespace Plato.Internal.Data.Migrations
{
    public class DataMigrationBuilder : IDataMigrationBuilder
    {
    
        private List<PreparedMigration> _schemas;
        private MigrationType _migrationType;

        private readonly IEnumerable<IMigrationProvider> _migrationProviders;
        private readonly IDataMigrationManager _dataMigrationManager;

        public MigrationType DataMigrationType => _migrationType;

        public DataMigrationBuilder(
            IEnumerable<IMigrationProvider> migrationProviders,
            IDataMigrationManager dataMigrationManager)
        {
            _migrationProviders = migrationProviders;
            _dataMigrationManager = dataMigrationManager;
        }

        #region "Implementation"

        public IDataMigrationBuilder BuildMigrations(List<string> versions)
        {

            foreach (var provider in _migrationProviders)
            {
                _schemas = provider.LoadSchemas(versions).Schemas;
            }
         
            if (_schemas?.Count > 0)
            {
                DetectMigrationType();
            }
            return this;
        }

        public async Task<DataMigrationResult> ApplyMigrationsAsync()
        {
            if (_schemas?.Count > 0)
            {
                var dataMigrationRecord = BuildDataMigrationRecord();
                return await _dataMigrationManager.ApplyMigrationsAsync(dataMigrationRecord);

            }
            return default(DataMigrationResult);
        }

        #endregion

        #region "Private Methods"

        DataMigrationRecord BuildDataMigrationRecord()
        {
            if (_schemas == null)
                return null;
            var migrations = new DataMigrationRecord();
            foreach (var schema in _schemas)
            {
                migrations.Migrations.Add(new DataMigration()
                {
                    Version = schema.Version,
                    Statements = schema.Statements
                });
            }
            return migrations;
        }

     
        void DetectMigrationType()
        {
            var first = _schemas[0];
            var last = _schemas[_schemas.Count - 1];

            // we have more than one version
            if (first.Version != last.Version)
            {
                // get higher versions
                var higherVersions =
                    (from s in _schemas
                     where s.TypedVersion > first.TypedVersion
                        select s).ToList();
                // get lower versions
                var lowerVersions =
                    (from s in _schemas
                     where s.TypedVersion < first.TypedVersion
                        select s).ToList();
                if (higherVersions.Count > 0)
                {
                    _migrationType = MigrationType.Upgrade;
                    return;
                }
                if (lowerVersions.Count > 0)
                {
                    _migrationType = MigrationType.Rollback;
                    return;
                }
            }
            _migrationType = MigrationType.Install;
        }

        #endregion
        
    }

}
