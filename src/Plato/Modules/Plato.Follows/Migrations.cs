using System.Data;
using System.Collections.Generic;
using Plato.Internal.Data.Migrations.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;

namespace Plato.Follows
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

            // Add indexes to follows tables

            // Follows table
            var follows = new SchemaTable()
            {
                Name = "Follows",
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
                        Name = "[Name]",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ThingId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CancellationToken",
                        DbType = DbType.String,
                        Length = "100"
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset
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
                    });

                // TODO: Add to FeatureEventHandlers
                builder.IndexBuilder.CreateIndex(new SchemaIndex()
                {
                    TableName = follows.Name,
                    Columns = new string[]
                    {
                        "Name",
                        "ThingId",
                        "CreatedUserId",
                        "CreatedDate"
                    }
                });

                // Add builder results to output
                output.AddRange(builder.Statements);

            }

            return output;

        }

    }

}
