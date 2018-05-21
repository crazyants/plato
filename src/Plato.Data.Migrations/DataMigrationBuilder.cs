using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plato.Data.Abstractions.Schemas;

namespace Plato.Data.Migrations
{
    public class DataMigrationBuilder : IDataMigrationBuilder
    {
    
        private List<PreparedSchema> _parsedSchemas;
        private MigrationType _migrationType;

        private readonly ISchemaProvider _schemaProvider;
        private readonly IDataMigrationManager _dataMigrationManager;

        public MigrationType DataMigrationType => _migrationType;

        public DataMigrationBuilder(
            ISchemaProvider schemaProvider,
            IDataMigrationManager dataMigrationManager)
        {
            _schemaProvider = schemaProvider;
            _dataMigrationManager = dataMigrationManager;
        }

        #region "Implementation"

        public void BuildMigrations(List<string> versions)
        {
            _parsedSchemas = _schemaProvider.LoadSchemas(versions).Schemas;
            if (_parsedSchemas?.Count > 0)
            {
                DetectMigrationType();
                _dataMigrationManager.ApplyMigrations(BuildDataMigration());
            }
        }

        #endregion

        #region "Private Methods"

        DataMigrationRecord BuildDataMigration()
        {
            if (_parsedSchemas == null)
                return null;
            var migrations = new DataMigrationRecord();
            foreach (var schema in _parsedSchemas)
            {
                migrations.Migrations.Add(new DataMigration()
                {
                    Version = schema.Version,
                    Statements = PrepareStatements(schema)
                });
            }
            return migrations;
        }

        List<string> PrepareStatements(PreparedSchema schema)
        {
            var statements = new List<string>();
            switch (this.DataMigrationType)
            {
                case MigrationType.Install:
                    statements = schema.InstallStatements;
                    break;
                case MigrationType.Upgrade:
                    statements = schema.UpgradeStatements;
                    break;
                case MigrationType.Rollback:
                    statements = schema.RollbackStatements;
                    break;
            }
            return statements;
        }

        void DetectMigrationType()
        {
            var first = _parsedSchemas[0];
            var last = _parsedSchemas[_parsedSchemas.Count - 1];
            if (first.Version != last.Version)
            {
                // get higer versions
                var higherVersions =
                    (from s in _parsedSchemas
                     where s.TypedVersion > first.TypedVersion
                        select s).ToList();
                // get lower versins
                var lowerVersions =
                    (from s in _parsedSchemas
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
