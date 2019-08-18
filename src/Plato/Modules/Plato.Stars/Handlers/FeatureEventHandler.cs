using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Stars.Handlers
{
    public class FeatureEventHandler : BaseFeatureEventHandler
    {

        public string Version { get; } = "1.0.0";

        // Follows table
        private readonly SchemaTable _stars = new SchemaTable()
        {
            Name = "Stars",
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

                // Configure
                Configure(builder);

                // Stars
                Stars(builder);
                
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

                // Drop tables
                builder.TableBuilder.DropTable(_stars);

                // Drop procedures
                builder.ProcedureBuilder
                    .DropDefaultProcedures(_stars)
                    .DropProcedure(new SchemaProcedure("SelectStarsPaged"))
                    .DropProcedure(new SchemaProcedure("SelectStarsByNameAndThingId"))
                    .DropProcedure(new SchemaProcedure("SelectStarByNameThingIdAndCreatedUserId"));
                
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

        void Stars(ISchemaBuilder builder)
        {
            
            // Tables
            builder.TableBuilder.CreateTable(_stars);

            // Procedures
            builder.ProcedureBuilder
                .CreateDefaultProcedures(_stars)

                // Overwrite our SelectFollowById created via CreateDefaultProcedures
                // above to also return basic user data with follow
                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectStarById",
                            @"SELECT s.*, 
                                u.Email, 
                                u.UserName, 
                                u.DisplayName, 
                                u.NormalizedUserName 
                                FROM {prefix}_Stars s WITH (nolock) 
                                LEFT OUTER JOIN {prefix}_Users u ON s.CreatedUserId = u.Id 
                                WHERE (
                                    s.Id = @Id 
                                )")
                        .ForTable(_stars)
                        .WithParameter(_stars.PrimaryKeyColumn))


                .CreateProcedure(
                    new SchemaProcedure("SelectStarsByNameAndThingId",
                            @"SELECT s.*, 
                                u.Email, 
                                u.UserName, 
                                u.DisplayName, 
                                u.NormalizedUserName 
                                FROM {prefix}_Stars s WITH (nolock) 
                                LEFT OUTER JOIN {prefix}_Users u ON s.CreatedUserId = u.Id 
                                WHERE (
                                    (s.[Name] = @Name AND s.ThingId = @ThingId) AND
                                    (u.EmailConfirmed = 1 AND u.LockoutEnabled = 0)
                                )")
                        .ForTable(_stars)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "[Name]",
                                DbType = DbType.String,
                                Length = "255"
                            },
                            new SchemaColumn()
                            {
                                Name = "ThingId",
                                DbType = DbType.Int32,
                            }
                        }))

                .CreateProcedure(
                    new SchemaProcedure("SelectStarByNameThingIdAndCreatedUserId",
                            @"SELECT s.*, 
                                u.Email, 
                                u.UserName, 
                                u.DisplayName, 
                                u.NormalizedUserName 
                                FROM {prefix}_Stars s WITH (nolock) 
                                LEFT OUTER JOIN {prefix}_Users u ON s.CreatedUserId = u.Id 
                                WHERE (
                                    s.[Name] = @Name AND s.ThingId = @ThingId AND u.Id = @CreatedUserId
                                )")
                        .ForTable(_stars)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "[Name]",
                                DbType = DbType.String,
                                Length = "255"
                            },
                            new SchemaColumn()
                            {
                                Name = "ThingId",
                                DbType = DbType.Int32,
                            },
                            new SchemaColumn()
                            {
                                Name = "CreatedUserId",
                                DbType = DbType.Int32,
                            }
                        }))

                .CreateProcedure(new SchemaProcedure("SelectStarsPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_stars)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "[Name]",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _stars.Name,
                Columns = new string[]
                {
                    "Name",
                    "ThingsId",
                    "CreatedUserId"
                }
            });
            
        }

        #endregion

    }

}
