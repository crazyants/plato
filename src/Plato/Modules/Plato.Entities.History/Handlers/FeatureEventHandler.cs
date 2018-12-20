using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Entities.History.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {

        public string Version { get; } = "1.0.0";

        // EntityHistory table
        private readonly SchemaTable _entityHistory = new SchemaTable()
        {
            Name = "EntityHistory",
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
                    Name = "[Message]",
                    DbType = DbType.String,
                    Length = "max"
                },
                new SchemaColumn()
                {
                    Name = "MajorVersion",
                    DbType = DbType.Int16
                },
                new SchemaColumn()
                {
                    Name = "MinorVersion",
                    DbType = DbType.Int16
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

                // Entity history
                EntityHistory(builder);

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

                // drop entity history
                builder
                    .DropTable(_entityHistory)
                    .DropDefaultProcedures(_entityHistory)
                    .DropProcedure(new SchemaProcedure("SelectEntityHistoryPaged"));

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

        void EntityHistory(ISchemaBuilder builder)
        {

            builder
                .CreateTable(_entityHistory)
                .CreateDefaultProcedures(_entityHistory);

            // Overwrite our SelectFollowById created via CreateDefaultProcedures
            // above to also return basic user data with follow
            builder.CreateProcedure(
                new SchemaProcedure(
                        $"SelectEntityHistoryById",
                        @"SELECT h.*, 
                                u.Email, 
                                u.UserName, 
                                u.DisplayName, 
                                u.NormalizedUserName,
                                u.FirstName,
                                u.LastName,
                                u.Alias
                                FROM {prefix}_EntityHistory h WITH (nolock) 
                                LEFT OUTER JOIN {prefix}_Users u ON h.CreatedUserId = u.Id 
                                WHERE (
                                    h.Id = @Id 
                                )")
                    .ForTable(_entityHistory)
                    .WithParameter(_entityHistory.PrimaryKeyColumn));
            
            builder.CreateProcedure(new SchemaProcedure("SelectEntityHistoryPaged", StoredProcedureType.SelectPaged)
                .ForTable(_entityHistory)
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
