﻿using System.Collections.Generic;
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
                Follows(builder);
                
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
                    .DropTable(_stars)
                    .DropDefaultProcedures(_stars)
                    .DropProcedure(new SchemaProcedure("SelectFollowsPaged"))
                    .DropProcedure(new SchemaProcedure("SelectFollowsByNameAndThingId"))
                    .DropProcedure(new SchemaProcedure("SelectFollowByNameThingIdAndCreatedUserId"));
                
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

        void Follows(ISchemaBuilder builder)
        {
            
            builder
                .CreateTable(_stars)
                .CreateDefaultProcedures(_stars);
            
            // Overwrite our SelectFollowById created via CreateDefaultProcedures
            // above to also return basic user data with follow
            builder.CreateProcedure(
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
                    .WithParameter(_stars.PrimaryKeyColumn));
                    
            builder
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
                        }));
            
            builder
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
                        }));
            
            builder.CreateProcedure(new SchemaProcedure("SelectStarsPaged", StoredProcedureType.SelectPaged)
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

        }
        
        #endregion
        
    }

}