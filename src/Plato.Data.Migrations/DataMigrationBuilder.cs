using System;
using System.Collections.Generic;
using System.Text;
using Plato.Data.Schemas;

namespace Plato.Data.Migrations
{
    public class DataMigrationBuilder : IDataMigrationBuilder
    {

        private List<string> _versions;

        private readonly ISchemaLoader _schemaLoader;

        public DataMigrationBuilder(
            ISchemaLoader schemaLoader)
        {
            _schemaLoader = schemaLoader;
        }

        public void BuildMigrations(List<string> versions)
        {

            _versions = versions;
            // load schemas
            _schemaLoader.LoadSchemas(_versions);

            if (_schemaLoader.LoadedSchemas != null)
            {

            }

        }

    }
}
