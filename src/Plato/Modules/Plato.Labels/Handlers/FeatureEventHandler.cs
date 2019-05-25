using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Labels.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {
    
        public string Version { get; } = "1.0.0";
        
        // Labels table
        private readonly SchemaTable _labels = new SchemaTable()
        {
            Name = "Labels",
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
                        Name = "ParentId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "FeatureId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "[Name]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Description]",
                        Length = "500",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Alias",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "IconCss",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ForeColor",
                        Length = "50",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "BackColor",
                        Length = "50",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "SortOrder",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalEntities",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalFollows",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalViews",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "LastEntityId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "LastEntityDate",
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
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    }
                }
        };

        // Label data table
        private readonly SchemaTable _labelData = new SchemaTable()
        {
            Name = "LabelData",
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
                    Name = "LabelId",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "[Key]",
                    Length = "255",
                    DbType = DbType.String
                },
                new SchemaColumn()
                {
                    Name = "[Value]",
                    Length = "max",
                    DbType = DbType.String
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
                },
                new SchemaColumn()
                {
                    Name = "ModifiedUserId",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "ModifiedDate",
                    DbType = DbType.DateTimeOffset,
                    Nullable = true
                }
            }
        };

        // Label Roles table
        private readonly SchemaTable _labelRoles = new SchemaTable()
        {
            Name = "LabelRoles",
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
                        Name = "LabelId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "RoleId",
                        DbType = DbType.Int32
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
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    }
                }
        };

        // Entity Labels table
        private readonly SchemaTable _entityLabels = new SchemaTable()
        {
            Name = "EntityLabels",
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
                    Name = "LabelId",
                    DbType = DbType.Int32
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
                },
                new SchemaColumn()
                {
                    Name = "ModifiedUserId",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "ModifiedDate",
                    DbType = DbType.DateTimeOffset,
                    Nullable = true
                }
            }
        };
        
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public FeatureEventHandler(
            ISchemaBuilder schemaBuilder,
            ISchemaManager schemaManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
        }
        
        #region "Implementation"

        public override async Task InstallingAsync(IFeatureEventContext context)
        {

            if (context.Logger.IsEnabled(LogLevel.Information))
                context.Logger.LogInformation($"InstallingAsync called within {ModuleId}");
            
            //var schemaBuilder = context.ServiceProvider.GetRequiredService<ISchemaBuilder>();
            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // Labels schema
                Labels(builder);

                // Label data
                LabelData(builder);

                // Label roles schema
                LabelRoles(builder);

                // Entity labels schema
                EntityLabels(builder);
                
                // Log statements to execute
                if (context.Logger.IsEnabled(LogLevel.Information))
                {
                    context.Logger.LogInformation($"The following SQL statements will be executed...");
                    foreach (var statement in builder.Statements)
                    {
                        context.Logger.LogInformation(statement);
                    }
                }

                // Execute statements
                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    context.Errors.Add(error, $"InstallingAsync within {this.GetType().FullName}");
                }
          

            }
            
        }

        public override Task InstalledAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override async Task UninstallingAsync(IFeatureEventContext context)
        {

            if (context.Logger.IsEnabled(LogLevel.Information))
                context.Logger.LogInformation($"UninstallingAsync called within {ModuleId}");
            
            using (var builder = _schemaBuilder)
            {
                
                // drop labels
                builder.TableBuilder.DropTable(_labels);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_labels)
                    .DropProcedure(new SchemaProcedure("SelectLabelsPaged", StoredProcedureType.SelectByKey))
                    .DropProcedure(new SchemaProcedure("SelectLabelsByFeatureId", StoredProcedureType.SelectByKey));
                
                // drop Label data
                builder.TableBuilder.DropTable(_labelData);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_labelData)
                    .DropProcedure(new SchemaProcedure("SelectLabelDatumByLabelId"))
                    .DropProcedure(new SchemaProcedure("SelectLabelDatumPaged"));
                
                // drop Label roles
                builder.TableBuilder.DropTable(_labelRoles);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_labelRoles)
                    .DropProcedure(new SchemaProcedure("SelectLabelRolesByLabelId"))
                    .DropProcedure(new SchemaProcedure("SelectLabelRolesPaged"))
                    .DropProcedure(new SchemaProcedure("DeleteLabelRolesByLabelId"))
                    .DropProcedure(new SchemaProcedure("DeleteLabelRolesByRoleIdAndLabelId"));
                
                // drop entity labels
                builder.TableBuilder.DropTable(_entityLabels);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_entityLabels)
                    .DropProcedure(new SchemaProcedure("SelectEntityLabelsByEntityId"))
                    .DropProcedure(new SchemaProcedure("DeleteEntityLabelsByEntityId"))
                    .DropProcedure(new SchemaProcedure("DeleteEntityLabelByEntityIdAndLabelId"))
                    .DropProcedure(new SchemaProcedure("SelectEntityLabelsPaged"));
                
                // Log statements to execute
                if (context.Logger.IsEnabled(LogLevel.Information))
                {
                    context.Logger.LogInformation($"The following SQL statements will be executed...");
                    foreach (var statement in builder.Statements)
                    {
                        context.Logger.LogInformation(statement);
                    }
                }

                // Execute statements
                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    context.Logger.LogCritical(error, $"An error occurred within the UninstallingAsync method within {this.GetType().FullName}");
                    context.Errors.Add(error, $"UninstallingAsync within {this.GetType().FullName}");
                }
          

            }
            
        }

        public override Task UninstalledAsync(IFeatureEventContext context)
        {

            return Task.CompletedTask;
        }

        #endregion

        #region "Private Methods"

        void Configure(ISchemaBuilder builder)
        {

            builder
                .Configure(options =>
                {
                    options.ModuleName = ModuleId;
                    options.Version = Version;
                    options.DropTablesBeforeCreate = true;
                    options.DropProceduresBeforeCreate = true;
                });

        }

        void Labels(ISchemaBuilder builder)
        {

            builder.TableBuilder.CreateTable(_labels);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_labels)

                .CreateProcedure(new SchemaProcedure("SelectLabelsByFeatureId", StoredProcedureType.SelectByKey)
                    .ForTable(_labels)
                    .WithParameter(new SchemaColumn() {Name = "FeatureId", DbType = DbType.Int32}))

                .CreateProcedure(new SchemaProcedure("SelectLabelsPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_labels)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Keywords",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

        }

        void LabelData(ISchemaBuilder builder)
        {

            builder.TableBuilder.CreateTable(_labelData);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_labelData)
                .CreateProcedure(new SchemaProcedure("SelectLabelDatumByLabelId", StoredProcedureType.SelectByKey)
                    .ForTable(_labelData)
                    .WithParameter(new SchemaColumn() {Name = "LabelId", DbType = DbType.Int32}))

                .CreateProcedure(new SchemaProcedure("SelectLabelDatumPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_labelData)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "[Key]",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

        }
        
        void LabelRoles(ISchemaBuilder builder)
        {

            builder.TableBuilder.CreateTable(_labelRoles);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_labelRoles)

                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectLabelRoleById",
                            @" SELECT cr.*, r.[Name] AS RoleName
                                FROM {prefix}_LabelRoles cr WITH (nolock) 
                                    INNER JOIN {prefix}_Roles r ON cr.RoleId = r.Id                                    
                                WHERE (
                                   cr.Id = @Id
                                )")
                        .ForTable(_labelRoles)
                        .WithParameter(_labelRoles.PrimaryKeyColumn))

                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectLabelRolesByLabelId",
                            @" SELECT cr.*, r.[Name] AS RoleName
                                FROM {prefix}_LabelRoles cr WITH (nolock) 
                                    INNER JOIN {prefix}_Roles r ON cr.RoleId = r.Id                                    
                                WHERE (
                                   cr.LabelId = @LabelId
                                )")
                        .ForTable(_labelRoles)
                        .WithParameter(new SchemaColumn() {Name = "LabelId", DbType = DbType.Int32}))

                .CreateProcedure(new SchemaProcedure("DeleteLabelRolesByLabelId", StoredProcedureType.DeleteByKey)
                    .ForTable(_labelRoles)
                    .WithParameter(new SchemaColumn() {Name = "LabelId", DbType = DbType.Int32}))

                .CreateProcedure(new SchemaProcedure("DeleteLabelRolesByRoleIdAndLabelId",
                        StoredProcedureType.DeleteByKey)
                    .ForTable(_labelRoles)
                    .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn() {Name = "RoleId", DbType = DbType.Int32},
                            new SchemaColumn() {Name = "LabelId", DbType = DbType.Int32}
                        }
                    ))

                .CreateProcedure(new SchemaProcedure("SelectLabelRolesPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_labelRoles)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Keywords",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

        }

        void EntityLabels(ISchemaBuilder builder)
        {

            builder.TableBuilder.CreateTable(_entityLabels);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_entityLabels)

                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityLabelById",
                            @" SELECT el.*, l.[Name] AS LabelName
                                FROM {prefix}_Labels l WITH (nolock) 
                                    INNER JOIN {prefix}_EntityLabels el ON el.LabelId = l.Id                                    
                                WHERE (
                                   el.Id = @Id
                                )")
                        .ForTable(_labelRoles)
                        .WithParameter(_labelRoles.PrimaryKeyColumn))

                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityLabelsByEntityId",
                            @" SELECT el.*, l.[Name] AS LabelName
                                FROM {prefix}_Labels l WITH (nolock) 
                                    INNER JOIN {prefix}_EntityLabels el ON el.LabelId = l.Id                                    
                                WHERE (
                                   el.EntityId = @EntityId
                                )")
                        .ForTable(_entityLabels)
                        .WithParameter(new SchemaColumn() {Name = "EntityId", DbType = DbType.Int32}))

                .CreateProcedure(new SchemaProcedure("DeleteEntityLabelsByEntityId", StoredProcedureType.DeleteByKey)
                    .ForTable(_entityLabels)
                    .WithParameter(new SchemaColumn() {Name = "EntityId", DbType = DbType.Int32}))

                .CreateProcedure(new SchemaProcedure("DeleteEntityLabelByEntityIdAndLabelId",
                        StoredProcedureType.DeleteByKey)
                    .ForTable(_entityLabels)
                    .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn() {Name = "EntityId", DbType = DbType.Int32},
                            new SchemaColumn() {Name = "LabelId", DbType = DbType.Int32}
                        }
                    ))

                .CreateProcedure(new SchemaProcedure("SelectEntityLabelsPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_entityLabels)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "LabelId",
                            DbType = DbType.Int32,
                        },
                        new SchemaColumn()
                        {
                            Name = "EntityId",
                            DbType = DbType.Int32,
                        }
                    }));

        }
        
        #endregion

    }

}
