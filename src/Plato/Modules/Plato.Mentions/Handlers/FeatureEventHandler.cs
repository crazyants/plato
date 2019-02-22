using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Mentions.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {
        public string Version { get; } = "1.0.0";

        // EntityMentions table
        private readonly SchemaTable _entityMentions = new SchemaTable()
        {
            Name = "EntityMentions",
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
                        Name = "UserId",
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
        
        public override async Task InstallingAsync(IFeatureEventContext context)
        {
     
            //var schemaBuilder = context.ServiceProvider.GetRequiredService<ISchemaBuilder>();
            using (var builder = _schemaBuilder)
            {
                
                // Configure
                Configure(builder);

                // EntityMentions schema
                EntityMentions(builder);

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

                // drop EntityMentions
                builder.TableBuilder.DropTable(_entityMentions);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_entityMentions)
                    .DropProcedure(new SchemaProcedure("SelectEntityMentionsPaged"))
                    .DropProcedure(new SchemaProcedure("DeleteEntityMentionsByEntityId"))
                    .DropProcedure(new SchemaProcedure("DeleteEntityMentionsByEntityReplyId"));

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

        void EntityMentions(ISchemaBuilder builder)
        {

            builder.TableBuilder.CreateTable(_entityMentions);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_entityMentions)

                .CreateProcedure(new SchemaProcedure("SelectEntityMentionsPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_entityMentions)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "[Username]",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }))

                .CreateProcedure(new SchemaProcedure("DeleteEntityMentionsByEntityId", StoredProcedureType.DeleteByKey)
                    .ForTable(_entityMentions)
                    .WithParameter(new SchemaColumn() {Name = "EntityId", DbType = DbType.Int32}))

                .CreateProcedure(
                    new SchemaProcedure("DeleteEntityMentionsByEntityReplyId", StoredProcedureType.DeleteByKey)
                        .ForTable(_entityMentions)
                        .WithParameter(new SchemaColumn() {Name = "EntityReplyId", DbType = DbType.Int32}));


        }

    }

}
