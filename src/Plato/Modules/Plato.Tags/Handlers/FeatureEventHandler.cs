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
                        Name = "NameNormalized",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Description",
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
                        Name = "LastSeenDate",
                        DbType = DbType.DateTimeOffset
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
                    Name = "EntityReplyId",
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
                    DbType = DbType.DateTimeOffset
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
                
                // drop tags
                builder.TableBuilder.DropTable(_tags);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_tags)
                    .DropProcedure(new SchemaProcedure("SelectTagsPaged", StoredProcedureType.SelectByKey))
                    .DropProcedure(new SchemaProcedure("SelectTagsByFeatureId", StoredProcedureType.SelectByKey))
                    .DropProcedure(new SchemaProcedure("SelectTagByName", StoredProcedureType.SelectByKey))
                    .DropProcedure(new SchemaProcedure("SelectTagByNameNormalized", StoredProcedureType.SelectByKey));
                
                // drop entity tags
                builder.TableBuilder.DropTable(_entityTags);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_entityTags)
                    .DropProcedure(new SchemaProcedure("SelectEntityTagsByEntityId"))
                    .DropProcedure(new SchemaProcedure("SelectEntityTagsByEntityReplyId"))
                    .DropProcedure(new SchemaProcedure("DeleteEntityTagsByEntityId"))
                    .DropProcedure(new SchemaProcedure("DeleteEntityTagByEntityIdAndTagId"))
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

        void Tags(ISchemaBuilder builder)
        {

            builder.TableBuilder.CreateTable(_tags);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_tags)

            .CreateProcedure(new SchemaProcedure("SelectTagsByFeatureId", StoredProcedureType.SelectByKey)
                    .ForTable(_tags)
                    .WithParameter(new SchemaColumn() {Name = "FeatureId", DbType = DbType.Int32}))

            .CreateProcedure(new SchemaProcedure("SelectTagByName", StoredProcedureType.SelectByKey)
                    .ForTable(_tags)
                    .WithParameter(new SchemaColumn() { Name = "[Name]", DbType = DbType.String, Length = "255" }))

            .CreateProcedure(new SchemaProcedure("SelectTagByNameNormalized", StoredProcedureType.SelectByKey)
                    .ForTable(_tags)
                    .WithParameter(new SchemaColumn() { Name = "NameNormalized", DbType = DbType.String, Length = "255" }))
            
            .CreateProcedure(new SchemaProcedure("SelectTagsPaged", StoredProcedureType.SelectPaged)
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

            builder.TableBuilder.CreateTable(_entityTags);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_entityTags)

                // Overwrite default SelectEntityTagById
                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityTagById",
                            @"SELECT et.*, 
                                t.FeatureId,
                                t.[Name],
                                t.NameNormalized,    
                                t.Description,
                                t.Alias,
                                t.TotalEntities,
                                t.TotalFollows,
                                t.LastSeenDate
                                FROM {prefix}_Tags t WITH (nolock) 
                                    INNER JOIN {prefix}_EntityTags et ON et.TagId = t.Id                                    
                                WHERE (
                                   et.Id = @Id
                                )")
                        .ForTable(_entityTags)
                        .WithParameter(_entityTags.PrimaryKeyColumn))

                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityTagsByEntityId",
                            @"SELECT et.*, 
                                t.FeatureId,
                                t.[Name],
                                t.NameNormalized,
                                t.Description,
                                t.Alias,
                                t.TotalEntities,
                                t.TotalFollows,
                                t.LastSeenDate
                                FROM {prefix}_Tags t WITH (nolock) 
                                    INNER JOIN {prefix}_EntityTags et ON et.TagId = t.Id                                    
                                WHERE (
                                   et.EntityId = @EntityId
                                )")
                        .ForTable(_entityTags)
                        .WithParameter(new SchemaColumn() {Name = "EntityId", DbType = DbType.Int32}))

                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityTagsByEntityReplyId",
                            @"SELECT et.*, 
                                t.FeatureId,
                                t.[Name],
                                t.NameNormalized,
                                t.Description,
                                t.Alias,
                                t.TotalEntities,
                                t.TotalFollows,
                                t.LastSeenDate
                                FROM {prefix}_Tags t WITH (nolock) 
                                    INNER JOIN {prefix}_EntityTags et ON et.TagId = t.Id                                    
                                WHERE (
                                   et.EntityReplyId = @EntityReplyId
                                )")
                        .ForTable(_entityTags)
                        .WithParameter(new SchemaColumn() {Name = "EntityReplyId", DbType = DbType.Int32}))

                .CreateProcedure(new SchemaProcedure("DeleteEntityTagsByEntityId", StoredProcedureType.DeleteByKey)
                    .ForTable(_entityTags)
                    .WithParameter(new SchemaColumn() {Name = "EntityId", DbType = DbType.Int32}))

                .CreateProcedure(new SchemaProcedure("DeleteEntityTagByEntityIdAndTagId",
                        StoredProcedureType.DeleteByKey)
                    .ForTable(_entityTags)
                    .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn() {Name = "EntityId", DbType = DbType.Int32},
                            new SchemaColumn() {Name = "TagId", DbType = DbType.Int32}
                        }
                    ))

                .CreateProcedure(new SchemaProcedure("SelectEntityTagsPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_entityTags)
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
        
        #endregion

    }

}
