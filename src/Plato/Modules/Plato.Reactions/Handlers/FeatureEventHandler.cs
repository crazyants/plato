using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Reactions.Handlers
{
    public class FeatureEventHandler : BaseFeatureEventHandler
    {

        public string Version { get; } = "1.0.0";

        // Reactions table
        private readonly SchemaTable _entityReactions = new SchemaTable()
        {
            Name = "EntityReactions",
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
                        Name = "ReactionName",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "Sentiment",
                        DbType = DbType.Int16
                    },
                    new SchemaColumn()
                    {
                        Name = "Points",
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

                // Reactions schema
                Reactions(builder);

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

                // drop emails
                builder
                    .DropTable(_entityReactions)
                    .DropDefaultProcedures(_entityReactions)
                    .DropProcedure(new SchemaProcedure("SelectEntityReactionsByEntityId"))
                    .DropProcedure(new SchemaProcedure("SelectEntityReactionsByUserIdAndEntityId"))
                    .DropProcedure(new SchemaProcedure("SelectEntityReactionsPaged"));

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

        void Reactions(ISchemaBuilder builder)
        {

            builder
                .CreateTable(_entityReactions)
                .CreateDefaultProcedures(_entityReactions);

            // Overwrite our SelectEntityReactionById created via CreateDefaultProcedures
            // above to also return simple user data with the reaction
            builder.CreateProcedure(
                new SchemaProcedure(
                        $"SelectEntityReactionById",
                        @" SELECT er.*, 
                                    u.UserName, 
                                    u.NormalizedUserName,
                                    u.DisplayName,
                                    u.FirstName,
                                    u.LastName,
                                    u.Alias                                   
                                FROM {prefix}_EntityReactions er WITH (nolock) 
                                    LEFT OUTER JOIN {prefix}_Users u ON er.CreatedUserId = u.Id                                    
                                WHERE (
                                   er.Id = @Id
                                )")
                    .ForTable(_entityReactions)
                    .WithParameter(_entityReactions.PrimaryKeyColumn));

            // Returns all reactions for a specific entity
            builder
                .CreateProcedure(
                    new SchemaProcedure("SelectEntityReactionsByEntityId",
                            @"SELECT er.*, 
                                    u.UserName, 
                                    u.NormalizedUserName,
                                    u.DisplayName,
                                    u.FirstName,
                                    u.LastName,
                                    u.Alias                                   
                                FROM {prefix}_EntityReactions er WITH (nolock) 
                                    LEFT OUTER JOIN {prefix}_Users u ON er.CreatedUserId = u.Id                                    
                                WHERE (
                                   er.EntityId = @EntityId
                                )")
                        .ForTable(_entityReactions)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "EntityId",
                            DbType = DbType.Int32
                        }));

            // Returns all reactions for the supplied UserId and EntityId
            builder
                .CreateProcedure(
                    new SchemaProcedure("SelectEntityReactionsByUserIdAndEntityId",
                            @"SELECT er.*, 
                                    u.UserName, 
                                    u.NormalizedUserName,
                                    u.DisplayName,
                                    u.FirstName,
                                    u.LastName,
                                    u.Alias                                   
                                FROM {prefix}_EntityReactions er WITH (nolock) 
                                    LEFT OUTER JOIN {prefix}_Users u ON er.CreatedUserId = u.Id                                    
                                WHERE (
                                    er.CreatedUserId = @UserId AND
                                    er.EntityId = @EntityId                                    
                                )")
                        .ForTable(_entityReactions)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "UserId",
                                DbType = DbType.Int32,
                            },
                            new SchemaColumn()
                            {
                                Name = "EntityId",
                                DbType = DbType.Int32,
                            }
                        }));


            builder.CreateProcedure(new SchemaProcedure("SelectEntityReactionsPaged", StoredProcedureType.SelectPaged)
                .ForTable(_entityReactions)
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
