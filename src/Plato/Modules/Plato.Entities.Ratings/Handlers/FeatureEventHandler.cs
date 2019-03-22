using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Entities.Ratings.Handlers
{
    public class FeatureEventHandler : BaseFeatureEventHandler
    {

        public string Version { get; } = "1.0.0";

        // Ratings table
        private readonly SchemaTable _entityRatings = new SchemaTable()
        {
            Name = "EntityRatings",
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
                        Name = "Rating",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "FeatureId",
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

        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public FeatureEventHandler(
            ISchemaManager schemaManager,
            ISchemaBuilder schemaBuilder)
        {
            _schemaManager = schemaManager;
            _schemaBuilder = schemaBuilder;
        }
        
        public override async Task InstallingAsync(IFeatureEventContext context)
        {

            if (context.Logger.IsEnabled(LogLevel.Information))
                context.Logger.LogInformation($"InstallingAsync called within {ModuleId}");

            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // Ratings schema
                Ratings(builder);
                
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
                // drop emails
                builder.TableBuilder.DropTable(_entityRatings);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_entityRatings)
                    .DropProcedure(new SchemaProcedure("SelectEntityRatingsByEntityId"))
                    .DropProcedure(new SchemaProcedure("SelectEntityRatingsByUserIdAndEntityId"))
                    .DropProcedure(new SchemaProcedure("SelectEntityRatingsPaged"));

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
        
        void Ratings(ISchemaBuilder builder)
        {
            
            builder.TableBuilder.CreateTable(_entityRatings);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_entityRatings)

                // Overwrite our SelectEntityReactionById created via CreateDefaultProcedures
                // above to also return simple user data with the reaction
                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityRatingById",
                            @" SELECT er.*, 
                                    u.UserName,                              
                                    u.DisplayName,                                  
                                    u.Alias,
                                    u.PhotoUrl,
                                    u.PhotoColor
                                FROM {prefix}_EntityRatings er WITH (nolock) 
                                    LEFT OUTER JOIN {prefix}_Users u ON er.CreatedUserId = u.Id                                    
                                WHERE (
                                   er.Id = @Id
                                )")
                        .ForTable(_entityRatings)
                        .WithParameter(_entityRatings.PrimaryKeyColumn))

                // Returns all ratings for a specific entity
                .CreateProcedure(
                    new SchemaProcedure("SelectEntityRatingsByEntityId",
                            @"SELECT er.*, 
                                    u.UserName,                               
                                    u.DisplayName,                                 
                                    u.Alias,
                                    u.PhotoUrl,
                                    u.PhotoColor
                                FROM {prefix}_EntityRatings er WITH (nolock) 
                                    LEFT OUTER JOIN {prefix}_Users u ON er.CreatedUserId = u.Id                                    
                                WHERE (
                                   er.EntityId = @EntityId
                                )")
                        .ForTable(_entityRatings)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "EntityId",
                            DbType = DbType.Int32
                        }))

                // Returns all ratings for the supplied UserId and EntityId
                .CreateProcedure(
                    new SchemaProcedure("SelectEntityRatingsByUserIdAndEntityId",
                            @"SELECT er.*, 
                                    u.UserName, 
                                    u.NormalizedUserName,
                                    u.DisplayName,                                    
                                    u.Alias,
                                    u.PhotoUrl,
                                    u.PhotoColor
                                FROM {prefix}_EntityRatings er WITH (nolock) 
                                    LEFT OUTER JOIN {prefix}_Users u ON er.CreatedUserId = u.Id                                    
                                WHERE (
                                    er.CreatedUserId = @UserId AND
                                    er.EntityId = @EntityId                                    
                                )")
                        .ForTable(_entityRatings)
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
                        }))

                .CreateProcedure(new SchemaProcedure("SelectEntityRatingsPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_entityRatings)
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
        
    }

}
