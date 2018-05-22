using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;
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
        }

        #endregion

        #region "Private Methods"

        public DataMigrationResult InitialMigration()
        {

            _schemaBuilder.CreateTable("Settings", new List<SchemaColumn>()
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
            });

            _schemaBuilder.Execute();




            return _migrationBuilder.BuildMigrations(
                new List<string>()
                {
                    "1.0.0"
                }).ApplyMigrations();

        }

        #endregion




    }
}
