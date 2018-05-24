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

            using (var builder = _schemaBuilder)
            {
                // Settings

                var settingsTable = new SchemaTable()
                {
                    Name = "Settings",
                    Columns = new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            PrimaryKey = true,
                            Name = "Id",
                            DbType = DbType.Int32
                        },
                        new SchemaColumn()
                        {
                            Name = "[Key]",
                            Length = "255",
                            DbType = DbType.String,

                        },
                        new SchemaColumn()
                        {
                            Name = "[Value]",
                            Length = "max",
                            DbType = DbType.String,

                        },
                        new SchemaColumn()
                        {
                            Name = "CreatedDate",
                            DbType = DbType.DateTime2
                        },
                        new SchemaColumn()
                        {
                            Name = "CreatedUserId",
                            DbType = DbType.Int32
                        },
                        new SchemaColumn()
                        {
                            Name = "ModifiedDate",
                            DbType = DbType.DateTime2
                        },
                        new SchemaColumn()
                        {
                            Name = "ModifiedUserId",
                            DbType = DbType.Int32
                        }
                    }
                };

                builder
                    .Configure(options => options.Version = "1.0.0")
                    .CreateTable(settingsTable)
                    .CreateDefaultProcedures(settingsTable)
                    .CreateProcedure(new SchemaProcedure("SelectSettingByKey", StoredProcedureType.SelectByKey).ForTable(settingsTable).WithKey(new SchemaColumn() {Name = "[Key]", DbType = DbType.Int32}));
                    
                
                var result = builder.Apply();

                return new DataMigrationResult()
                {
                    Errors = result.Errors
                };
            }
            
            //return _migrationBuilder.BuildMigrations(
            //    new List<string>()
            //    {
            //        "1.0.0"
            //    }).ApplyMigrations();

        }

        #endregion




    }
}
