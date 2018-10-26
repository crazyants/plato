using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Follow.Handlers
{
    public class FeatureEventHandler : BaseFeatureEventHandler
    {

        public string Version { get; } = "1.0.0";

        // EntityFollows table
        private readonly SchemaTable _entityFollows = new SchemaTable()
        {
            Name = "EntityFollows",
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
                        Name = "UserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CancellationGuid",
                        DbType = DbType.String,
                        Length = "100"
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset
                    },
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

                // Entity follows
                EntityFollows(builder);
                
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

                // drop entity follows
                builder
                    .DropTable(_entityFollows)
                    .DropDefaultProcedures(_entityFollows)
                    .DropProcedure(new SchemaProcedure("SelectEntityFollowsPaged"))
                    .DropProcedure(new SchemaProcedure("SelectEntityFollowsByEntityId"))
                    .DropProcedure(new SchemaProcedure("SelectEntityFollowsByUserIdAndEntityId"));
                
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

        void EntityFollows(ISchemaBuilder builder)
        {
            
            builder
                .CreateTable(_entityFollows)
                .CreateDefaultProcedures(_entityFollows);
            
            // Overwrite our SelectEntityFollowById created via CreateDefaultProcedures
            // above to also return basic user data with follow
            builder.CreateProcedure(
                new SchemaProcedure(
                        $"SelectEntityFollowById",
                        @"SELECT f.*, 
                                u.Email, 
                                u.UserName, 
                                u.DisplayName, 
                                u.NormalizedUserName 
                                FROM {prefix}_EntityFollows f WITH (nolock) 
                                LEFT OUTER JOIN {prefix}_Users u ON f.UserId = u.Id 
                                WHERE (
                                    f.Id = @Id 
                                )")
                    .ForTable(_entityFollows)
                    .WithParameter(_entityFollows.PrimaryKeyColumn));
                    
            // Returns all followers for a specific entity
            builder
                .CreateProcedure(
                    new SchemaProcedure("SelectEntityFollowsByEntityId",
                            @"SELECT f.*, 
                                u.Email, 
                                u.UserName, 
                                u.DisplayName, 
                                u.NormalizedUserName 
                                FROM {prefix}_EntityFollows f WITH (nolock) 
                                LEFT OUTER JOIN {prefix}_Users u ON f.UserId = u.Id 
                                WHERE (
                                    f.EntityId = @EntityId AND
                                    u.EmailConfirmed = 1 AND 
                                    u.LockoutEnabled = 0
                                )")
                        .ForTable(_entityFollows)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "EntityId",
                            DbType = DbType.Int32
                        }));

            // Returns a follow based on the supplied UserId and EntityId
            builder
                .CreateProcedure(
                    new SchemaProcedure("SelectEntityFollowsByUserIdAndEntityId",
                            @"SELECT f.*, 
                                u.Email, 
                                u.UserName, 
                                u.DisplayName, 
                                u.NormalizedUserName 
                                FROM {prefix}_EntityFollows f WITH (nolock) 
                                LEFT OUTER JOIN {prefix}_Users u ON f.UserId = u.Id 
                                WHERE (
                                    f.EntityId = @EntityId AND
                                    u.Id = @UserId
                                )")
                        .ForTable(_entityFollows)
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
            
            builder.CreateProcedure(new SchemaProcedure("SelectEntityFollowsPaged", StoredProcedureType.SelectPaged)
                .ForTable(_entityFollows)
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
