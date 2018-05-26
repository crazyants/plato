using System;
using System.Collections.Generic;
using System.Data;
using Plato.Data.Abstractions.Schemas;

namespace Plato.Data.Migrations
{

    public class AutomaticDataMigrations
    {

        private readonly IDataMigrationBuilder _migrationBuilder;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IDataMigrationManager _migrationManager;

        #region "Constructor"

        public AutomaticDataMigrations(
            IDataMigrationBuilder migrationBuilder,
            IDataMigrationManager migrationManager,
            ISchemaBuilder schemaBuilder)
        {
            _migrationBuilder = migrationBuilder;
            _schemaBuilder = schemaBuilder;
            _migrationManager = migrationManager;
        }

        #endregion

        #region "Private Methods"

        public DataMigrationResult InitialMigration()
        {


            return null;
            //return _migrationBuilder.BuildMigrations(
            //    new List<string>()
            //    {
            //        "1.0.0"
            //    }).ApplyMigrations();

        }
        
        #endregion




    }
}
