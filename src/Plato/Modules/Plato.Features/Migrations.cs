using System.Data;
using System.Collections.Generic;
using Plato.Internal.Data.Migrations.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;

namespace Plato.Features
{
    public class Migrations : BaseMigrationProvider
    {

        private readonly ISchemaBuilder _schemaBuilder;

        public Migrations(ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;

            AvailableMigrations = new List<PreparedMigration>
            {
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.0.1",
                    Statements = v_1_0_1()
                }
            };

        }

        public ICollection<string> v_1_0_1()
        {

            // Add indexes to ShellFeatures

            var shellFeatures = new SchemaTable()
            {
                Name = "ShellFeatures",
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
                        Name = "ModuleId",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "[Version]",
                        DbType = DbType.String,
                        Length = "10"
                    },
                    new SchemaColumn()
                    {
                        Name = "Settings",
                        DbType = DbType.String,
                        Length = "max"
                    }
                }
            };

            var output = new List<string>();

            using (var builder = _schemaBuilder)
            {

                builder
                    .Configure(options =>
                    {
                        options.ModuleName = ModuleId;
                        options.Version = "1.0.1";
                        options.DropProceduresBeforeCreate = true;
                    });


                // TODO: Add to FeatureEventHandlers
                builder.IndexBuilder.CreateIndex(new SchemaIndex()
                {
                    TableName = shellFeatures.Name,
                    Columns = new string[]
                    {
                        "ModuleId"
                    }
                });

                // Add builder results to output
                output.AddRange(builder.Statements);

            }

            return output;

        }

    }

}
