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
                    Name = "Html",
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

                // drop entity history
                builder.TableBuilder.DropTable(_entityHistory);

                builder.ProcedureBuilder
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

        void EntityHistory(ISchemaBuilder builder)
        {

            builder.TableBuilder.CreateTable(_entityHistory);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_entityHistory)

                // Overwrite our SelectFollowById created via CreateDefaultProcedures
                // above to also return basic user data with follow
                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityHistoryById",
                            @"SELECT h.*, 
                                u.Email, 
                                u.UserName, 
                                u.DisplayName, 
                                u.Alias,
                                u.PhotoUrl,
                                u.PhotoColor
                            FROM {prefix}_EntityHistory h WITH (nolock) 
                                LEFT OUTER JOIN {prefix}_Users u ON h.CreatedUserId = u.Id 
                                WHERE (
                                    h.Id = @Id 
                                )")
                        .ForTable(_entityHistory)
                        .WithParameter(_entityHistory.PrimaryKeyColumn))

                // Overwrite InsertUpdateEntityHistory to calculate version numbers
                .CreateProcedure(
                    new SchemaProcedure(
                            $"InsertUpdateEntityHistory",
                            @"IF EXISTS (SELECT Id FROM {prefix}_EntityHistory WHERE (Id = @Id))
                        BEGIN

                           UPDATE {prefix}_EntityHistory SET 
                               EntityId = @EntityId,
                               EntityReplyId = @EntityReplyId,
                               [Message] = @Message,
                               Html = @Html,
                               MajorVersion = @MajorVersion,
                               MinorVersion = @MinorVersion,
                               CreatedUserId = @CreatedUserId,
                               CreatedDate = @CreatedDate
                               WHERE Id = @Id

                             SET @UniqueId = @Id;

                        END
                        ELSE
                        BEGIN

                            IF (@EntityReplyId = 0)
                            BEGIN
	                            SET @MajorVersion = (
                                    SELECT TOP 1 IsNull(MajorVersion, 0) FROM 
                                    {prefix}_EntityHistory WHERE EntityId = @EntityId
                                    ORDER BY Id DESC
	                            );
	                            SET @MinorVersion = (
                                    SELECT TOP 1 IsNull(MinorVersion, 0) FROM 
                                    {prefix}_EntityHistory WHERE EntityId = @EntityId
                                    ORDER BY Id DESC
	                            );
                            END
                            ELSE
                            BEGIN
                                SET @MajorVersion = (
                                    SELECT TOP 1 IsNull(MajorVersion, 0) FROM 
                                    {prefix}_EntityHistory WHERE EntityReplyId = @EntityReplyId
                                    ORDER BY Id DESC
	                            );
                                SET @MinorVersion = (
                                    SELECT TOP 1 IsNull(MinorVersion, 0) FROM 
                                    {prefix}_EntityHistory WHERE EntityReplyId = @EntityReplyId
                                    ORDER BY Id DESC
	                            );
                            END
                    
	                        IF (@MinorVersion <= 9)
		                        SET @MinorVersion = @MinorVersion + 1	
	                        IF (@MinorVersion = 10)
		                        SET @MinorVersion = 0;		
	                        IF (@MinorVersion = 0)
		                        SET @MajorVersion = (@MajorVersion + 1)
	                        
                            INSERT INTO {prefix}_EntityHistory ( 
                                EntityId,
                                EntityReplyId,
                                [Message],
                                Html,
                                MajorVersion,
                                MinorVersion,
                                CreatedUserId,
                                CreatedDate
                            ) VALUES (
                                @EntityId,
                                @EntityReplyId,
                                @Message,
                                @Html,
                                IsNull(@MajorVersion, 1),
                                IsNull(@MinorVersion, 0),
                                @CreatedUserId,
                                @CreatedDate
                            )

                             SET @UniqueId = SCOPE_IDENTITY();

                        END")
                        .ForTable(_entityHistory)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "Id",
                                DbType = DbType.Int32,
                            },
                            new SchemaColumn()
                            {
                                Name = "EntityId",
                                DbType = DbType.Int32,
                            },
                            new SchemaColumn()
                            {
                                Name = "EntityReplyId",
                                DbType = DbType.Int32,
                            },
                            new SchemaColumn()
                            {
                                Name = "[Message]",
                                DbType = DbType.String,
                                Length = "max"
                            },
                            new SchemaColumn()
                            {
                                Name = "Html",
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
                            },
                            new SchemaColumn()
                            {
                                Name = "UniqueId",
                                DbType = DbType.Int32,
                                Direction = Direction.Out
                            }
                        }))

                .CreateProcedure(new SchemaProcedure("SelectEntityHistoryPaged", StoredProcedureType.SelectPaged)
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
