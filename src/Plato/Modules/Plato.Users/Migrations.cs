using System;
using System.Linq;
using System.Collections.Generic;
using Plato.Internal.Data.Migrations;
using Plato.Internal.Data.Schemas.Abstractions;

namespace Plato.Users
{
    public class Migrations : IMigrationProvider
    {

        private readonly List<PreparedMigration> _migrations = new List<PreparedMigration>()
        {
            new PreparedMigration()
            {
                Version = "1.0.0",
                Statements = new List<string>()
                {
                    "ALTER {prefix}_Users DROP COLUMN "
                }
            }
        };
        
        private readonly ISchemaBuilder _schemaBuilder;

        public Migrations(ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
        }

        // -----

        public List<PreparedMigration> Schemas { get; } = new List<PreparedMigration>();

        public PreparedMigration GetSchema(string version)
        {
            return _migrations.FirstOrDefault(s => s.Version.Equals(version, StringComparison.OrdinalIgnoreCase));
        }

        public IMigrationProvider LoadSchemas(List<string> versions)
        {
            foreach (var migration in _migrations)
            {
                if (versions.Contains(migration.Version))
                {
                    Schemas.Add(migration);
                }
            }

            return this;
        }

    }

}
