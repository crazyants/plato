using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Notifications.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {
        public string Version { get; } = "1.0.0";

        // EntityMentions table
        private readonly SchemaTable _notifications = new SchemaTable()
        {
            Name = "Notifications",
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
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "UserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "[Text]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Url]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset
                    }
                }
        };
        
        #region "Constructor"

        private readonly ISchemaBuilder _schemaBuilder;

        public FeatureEventHandler(ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
        }

        #endregion

        #region "Implementation"

        public override async Task InstallingAsync(IFeatureEventContext context)
        {
            
            //var schemaBuilder = context.ServiceProvider.GetRequiredService<ISchemaBuilder>();
            using (var builder = _schemaBuilder)
            {

                // Configure
                Configure(builder);

                // Notifications schema
                Notifications(builder);

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

                // drop EntityMentions
                builder
                    .DropTable(_notifications)
                    .DropDefaultProcedures(_notifications)
                    .DropProcedure(new SchemaProcedure("SelectNotificationsPaged"));
                
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

        void Notifications(ISchemaBuilder builder)
        {

            builder
                .CreateTable(_notifications)
                .CreateDefaultProcedures(_notifications);

            builder.CreateProcedure(new SchemaProcedure("SelectNotificationsPaged", StoredProcedureType.SelectPaged)
                .ForTable(_notifications)
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
