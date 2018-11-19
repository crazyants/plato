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
        private readonly SchemaTable _userNotifications = new SchemaTable()
        {
            Name = "UserNotifications",
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
                        Name = "UserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "NotificationName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Title]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Message]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Url]",
                        Length = "500",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ReadDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
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

                // UserNotifications schema
                UserNotifications(builder);

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
                    .DropTable(_userNotifications)
                    .DropDefaultProcedures(_userNotifications)
                    .DropProcedure(new SchemaProcedure("SelectUserNotificationsPaged"));
                
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

        void UserNotifications(ISchemaBuilder builder)
        {

            builder
                .CreateTable(_userNotifications)
                .CreateDefaultProcedures(_userNotifications);

            builder.CreateProcedure(
                new SchemaProcedure(
                        $"SelectUserNotificationById",
                        @" SELECT un.*, 
                                    u.UserName, 
                                    u.NormalizedUserName,
                                    u.DisplayName,
                                    u.FirstName,
                                    u.LastName,
                                    u.Alias,  
                                    c.UserName AS CreatedUserName, 
                                    c.NormalizedUserName AS CreatedNormalizedUserName,
                                    c.DisplayName AS CreatedDisplayName,
                                    c.FirstName AS CreatedFirstName,
                                    c.LastName AS CreatedLastName,
                                    c.Alias AS CreatedAlias                           
                                FROM {prefix}_UserNotifications un WITH (nolock) 
                                    LEFT OUTER JOIN {prefix}_Users u ON un.UserId = u.Id
                                    LEFT OUTER JOIN {prefix}_Users c ON un.CreatedUserId = c.Id
                                WHERE (
                                   un.Id = @Id
                                )")
                    .ForTable(_userNotifications)
                    .WithParameter(_userNotifications.PrimaryKeyColumn));

            builder.CreateProcedure(new SchemaProcedure("SelectUserNotificationsPaged", StoredProcedureType.SelectPaged)
                .ForTable(_userNotifications)
                .WithParameters(new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        Name = "NotificationName",
                        DbType = DbType.String,
                        Length = "255"
                    }
                }));


        }

        #endregion


    }
}
