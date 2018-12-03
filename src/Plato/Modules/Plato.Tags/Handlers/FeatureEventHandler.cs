using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Tags.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {
    
        public string Version { get; } = "1.0.0";
        
        // Tags table
        private readonly SchemaTable _tags = new SchemaTable()
        {
            Name = "Tags",
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
                        Name = "Alias",
                        Length = "255",
                        DbType = DbType.String
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
        
        // Entity Tags table
        private readonly SchemaTable _entityTags = new SchemaTable()
        {
            Name = "EntityTags",
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
                    Name = "TagId",
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
                    DbType = DbType.DateTime
                }
            }
        };


        private readonly ISchemaBuilder _schemaBuilder;

        public FeatureEventHandler(ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
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

                // Tags schema
                Tags(builder);
                
                // Entity tags schema
                EntityTags(builder);
                
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
                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Errors.Add(error.Message, $"InstallingAsync within {this.GetType().FullName}");
                    }
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
                
                // drop tags
                builder
                    .DropTable(_tags)
                    .DropDefaultProcedures(_tags)
                    .DropProcedure(new SchemaProcedure("SelectTagsPaged", StoredProcedureType.SelectByKey))
                    .DropProcedure(new SchemaProcedure("SelectTagsByFeatureId", StoredProcedureType.SelectByKey));
                
                // drop entity tags
                builder
                    .DropTable(_entityTags)
                    .DropDefaultProcedures(_entityTags)
                    .DropProcedure(new SchemaProcedure("SelectEntityTagsByEntityId"))
                    .DropProcedure(new SchemaProcedure("DeleteEntityTagsByEntityId"))
                    .DropProcedure(new SchemaProcedure("DeleteEntityTaglByEntityIdAndTagId"))
                    .DropProcedure(new SchemaProcedure("SelectEntityTagsPaged"));
                
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
                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Logger.LogCritical(error.Message, $"An error occurred within the UninstallingAsync method within {this.GetType().FullName}");
                        context.Errors.Add(error.Message, $"UninstallingAsync within {this.GetType().FullName}");
                    }
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

        void Tags(ISchemaBuilder builder)
        {

            builder
                .CreateTable(_tags)
                .CreateDefaultProcedures(_tags);

            builder
                .CreateProcedure(new SchemaProcedure("SelectTagsByFeatureId", StoredProcedureType.SelectByKey)
                    .ForTable(_tags)
                    .WithParameter(new SchemaColumn() {Name = "FeatureId", DbType = DbType.Int32}));
            
            builder.CreateProcedure(new SchemaProcedure("SelectTagsPaged", StoredProcedureType.SelectPaged)
                .ForTable(_tags)
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
        
        void EntityTags(ISchemaBuilder builder)
        {

            builder
                .CreateTable(_entityTags)
                .CreateDefaultProcedures(_entityTags);

            builder.CreateProcedure(
                new SchemaProcedure(
                        $"SelectEntityTagById",
                        @" SELECT el.*, l.[Name] AS LabelName
                                FROM {prefix}_Labels l WITH (nolock) 
                                    INNER JOIN {prefix}_EntityLabels el ON el.LabelId = l.Id                                    
                                WHERE (
                                   el.Id = @Id
                                )")
                    .ForTable(_entityTags)
                    .WithParameter(_entityTags.PrimaryKeyColumn));

            builder.CreateProcedure(
                new SchemaProcedure(
                        $"SelectEntityTagsByEntityId",
                        @" SELECT el.*, l.[Name] AS LabelName
                                FROM {prefix}_Labels l WITH (nolock) 
                                    INNER JOIN {prefix}_EntityLabels el ON el.LabelId = l.Id                                    
                                WHERE (
                                   el.EntityId = @EntityId
                                )")
                    .ForTable(_entityTags)
                    .WithParameter(new SchemaColumn() { Name = "EntityId", DbType = DbType.Int32 }));

            builder
                .CreateProcedure(new SchemaProcedure("DeleteEntityLabelsByEntityId", StoredProcedureType.DeleteByKey)
                    .ForTable(_entityTags)
                    .WithParameter(new SchemaColumn() { Name = "EntityId", DbType = DbType.Int32 }));

            builder
                .CreateProcedure(new SchemaProcedure("DeleteEntityTagByEntityIdAndTagId",
                        StoredProcedureType.DeleteByKey)
                    .ForTable(_entityTags)
                    .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn() {Name = "EntityId", DbType = DbType.Int32},
                            new SchemaColumn() {Name = "TagId", DbType = DbType.Int32}
                        }
                    ));

            builder.CreateProcedure(new SchemaProcedure("SelectEntityLabelsPaged", StoredProcedureType.SelectPaged)
                .ForTable(_entityTags)
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
