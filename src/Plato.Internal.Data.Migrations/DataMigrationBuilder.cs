using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Migrations.Abstractions;

namespace Plato.Internal.Data.Migrations
{
    public class DataMigrationBuilder : IDataMigrationBuilder
    {
    
        private IList<PreparedMigration> _schemas;
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
        
        public IDataMigrationBuilder BuildMigrations(IList<string> versions)
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

        public IDataMigrationBuilder BuildMigrations(
            string moduleId, Version from, Version to)
        {

            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }
            
            // All versions between from and to
            var versions = from.GetVersionsBetween(to)?.ToList() ?? new List<Version>();

            // Add our final version if it's not already present
            if (!versions.Contains(to))
            {
                versions.Add(to);
            }

            // Build a string array of all versions to search
            var versionsToSearch = versions.Select(v => v.ToString()).ToArray();

            // Iterate all migration providers loading schemas for feature and versions
            foreach (var provider in _migrationProviders)
            {
                // Load all schemas for supplied module and versions
                _schemas = provider.LoadSchemas(versionsToSearch).Schemas
                    .Where(s => s.ModuleId.Equals(moduleId, StringComparison.OrdinalIgnoreCase)).ToList();
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
                    ModuleId = schema.ModuleId,
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
