using System.Data;
using System.Collections.Generic;
using Plato.Internal.Data.Migrations.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;

namespace Plato.Notifications
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

            var userNotifications = new SchemaTable()
            {
                Name = "UserNotifications",
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
                        Name = "UserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "NotificationName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Title]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Message]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Url]",
                        Length = "500",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ReadDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
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
                        options.DropIndexesBeforeCreate = true;
                    });

                // TODO: Add to FeatureEventHandlers
                builder.IndexBuilder.CreateIndex(new SchemaIndex()
                {
                    TableName = userNotifications.Name,
                    Columns = new string[]
                    {
                        "UserId",
                        "NotificationName",
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
