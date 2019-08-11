using System.Data;
using System.Collections.Generic;
using Plato.Internal.Data.Migrations.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;

namespace Plato.Entities.Metrics
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
                    Statements = v_1_0_2()
                },
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.0.2",
                    Statements = v_1_0_2()
                },
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.0.3",
                    Statements = v_1_0_3()
                }
            };

        }

        public ICollection<string> v_1_0_2()
        {

            var entityMetrics = new SchemaTable()
            {
                Name = "EntityMetrics",
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
                        Name = "EntityId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV4Address",
                        DbType = DbType.String,
                        Length = "20"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV6Address",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "UserAgent",
                        DbType = DbType.String,
                        Length = "255"
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
                        options.Version = "1.0.2";
                        options.DropProceduresBeforeCreate = true;
                    });

                // Drop & recreate SelectEntityMetricById stored procedure
                builder.ProcedureBuilder.CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityMetricById",
                            @" SELECT em.*, 
                                    e.Title,
                                    e.Alias,
                                    e.FeatureId,
                                    f.ModuleId,
                                    u.UserName,                              
                                    u.DisplayName,                                  
                                    u.Alias,
                                    u.PhotoUrl,
                                    u.PhotoColor
                                FROM {prefix}_EntityMetrics em WITH (nolock) 
                                    INNER JOIN {prefix}_Entities e ON em.EntityId = e.Id
                                    INNER JOIN {prefix}_ShellFeatures f ON e.FeatureId = f.Id
                                    LEFT OUTER JOIN {prefix}_Users u ON em.CreatedUserId = u.Id                                    
                                WHERE (
                                   em.Id = @Id
                                )")
                        .ForTable(entityMetrics)
                        .WithParameter(entityMetrics.PrimaryKeyColumn));

                // Add builder results to output
                output.AddRange(builder.Statements);

            }

            return output;

        }
        
        public ICollection<string> v_1_0_3()
        {

            var entityMetrics = new SchemaTable()
            {
                Name = "EntityMetrics",
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
                        Name = "EntityId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV4Address",
                        DbType = DbType.String,
                        Length = "20"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV6Address",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "UserAgent",
                        DbType = DbType.String,
                        Length = "255"
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
                        options.Version = "1.0.3";
                    });

                // TODO: Add to FeatureEventHandlers
                builder.IndexBuilder.CreateIndex(new SchemaIndex()
                {
                    TableName = entityMetrics.Name,
                    Columns = new string[]
                    {
                        "EntityId",
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
