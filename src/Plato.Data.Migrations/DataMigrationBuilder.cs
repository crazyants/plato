using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plato.Data.Abstractions.Schemas;

namespace Plato.Data.Migrations
{
    public class DataMigrationBuilder : IDataMigrationBuilder
    {
        public enum MigrationType
        {
            Install,
            Upgrade,
            Rollback
        }

        private List<string> _versions;
        private List<SchemaDescriptor> _loadedSchemas;
        private List<SchemaDescriptor> _internalSchemas;

        private MigrationType _migrationType;

        private readonly ISchemaLoader _schemaLoader;

        public DataMigrationBuilder(
            ISchemaLoader schemaLoader)
        {
            _schemaLoader = schemaLoader;
        }


        public void BuildMigrations(List<string> versions)
        {

            _versions = versions;

            _schemaLoader.LoadSchemas(_versions);
            _loadedSchemas = _schemaLoader.LoadedSchemas;

            PrepareInternalSchemas();

            if (_internalSchemas?.Count > 0)
            {
                SetInternalMigrationType();
                foreach (var schema in _loadedSchemas)
                {

                }
            }
            
        }

        private void PrepareInternalSchemas()
        {
            _internalSchemas = new List<SchemaDescriptor>();
            if (_loadedSchemas?.Count > 0)
            {
                foreach (var veraion in _versions)
                {
                    var foundVersion = _loadedSchemas.FirstOrDefault(s => s.Version == veraion);
                    AddToInternalSchema(foundVersion ?? new SchemaDescriptor() { Version = veraion });
                }
            }
        }

        void AddToInternalSchema(SchemaDescriptor schema)
        {
            schema.TypedVersion = GetTypedVersion(schema.Version);
            _internalSchemas.Add(schema);
        }

        void SetInternalMigrationType()
        {
            
            var first = _internalSchemas[0];
            var last = _internalSchemas[_internalSchemas.Count - 1];

            if (first.Version != last.Version)
            {
                // get higer versions
                var higherVersions =
                    (from s in _internalSchemas
                     where s.TypedVersion > first.TypedVersion
                        select s).ToList();
                // get lower versins
                var lowerVersions =
                    (from s in _internalSchemas
                     where s.TypedVersion < last.TypedVersion
                        select s).ToList();

                if (higherVersions.Count > 0)
                {
                    _migrationType = MigrationType.Upgrade;
                }
                if (lowerVersions.Count > 0)
                {
                    _migrationType = MigrationType.Rollback;
                }
            }
            
            _migrationType = MigrationType.Install;

        }

        Version GetTypedVersion(string input)
        {
            if (!Version.TryParse(input, out var version))
            {
                // Is is a single number?
                if (int.TryParse(input, out var major))
                    return new Version(major + 1, 0, 0);
            }
            if (version.Build != -1)
                return new Version(version.Major, version.Minor, version.Build + 1);
            if (version.Minor != -1)
                return new Version(version.Major, version.Minor + 1, 0);
            return version;
        }


    }
}
