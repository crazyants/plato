using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Users.Badges.Handlers
{

    //public class FeatureEventHandler : BaseFeatureEventHandler
    //{

    //    public string Version { get; } = "1.0.0";

    //    // EntityFollows table
    //    private readonly SchemaTable _userBadges = new SchemaTable()
    //    {
    //        Name = "UserBadges",
    //        Columns = new List<SchemaColumn>()
    //            {
    //                new SchemaColumn()
    //                {
    //                    PrimaryKey = true,
    //                    Name = "Id",
    //                    DbType = DbType.Int32
    //                },
    //                new SchemaColumn()
    //                {
    //                    Name = "BadgeName",
    //                    DbType = DbType.String,
    //                    Length = "255"
    //                },
    //                new SchemaColumn()
    //                {
    //                    Name = "UserId",
    //                    DbType = DbType.Int32
    //                },
    //                new SchemaColumn()
    //                {
    //                    Name = "CreatedDate",
    //                    DbType = DbType.DateTimeOffset
    //                }
    //            }
    //    };

    //    private readonly ISchemaBuilder _schemaBuilder;

    //    public FeatureEventHandler(ISchemaBuilder schemaBuilder)
    //    {
    //        _schemaBuilder = schemaBuilder;
    //    }

    //    #region "Implementation"

    //    public override async Task InstallingAsync(IFeatureEventContext context)
    //    {

    //        if (context.Logger.IsEnabled(LogLevel.Information))
    //            context.Logger.LogInformation($"InstallingAsync called within {ModuleId}");

    //        //var schemaBuilder = context.ServiceProvider.GetRequiredService<ISchemaBuilder>();
    //        using (var builder = _schemaBuilder)
    //        {

    //            // configure
    //            Configure(builder);

    //            // User badges
    //            UserBadges(builder);

    //            // Log statements to execute
    //            if (context.Logger.IsEnabled(LogLevel.Information))
    //            {
    //                context.Logger.LogInformation($"The following SQL statements will be executed...");
    //                foreach (var statement in builder.Statements)
    //                {
    //                    context.Logger.LogInformation(statement);
    //                }
    //            }

    //            // Execute statements
    //            var result = await builder.ApplySchemaAsync();
    //            if (result.Errors.Count > 0)
    //            {
    //                foreach (var error in result.Errors)
    //                {
    //                    context.Errors.Add(error.Message, $"InstallingAsync within {this.GetType().FullName}");
    //                }

    //            }

    //        }


    //    }

    //    public override Task InstalledAsync(IFeatureEventContext context)
    //    {
    //        return Task.CompletedTask;
    //    }

    //    public override async Task UninstallingAsync(IFeatureEventContext context)
    //    {
    //        if (context.Logger.IsEnabled(LogLevel.Information))
    //            context.Logger.LogInformation($"UninstallingAsync called within {ModuleId}");

    //        using (var builder = _schemaBuilder)
    //        {

    //            // drop user badges
    //            builder
    //                .DropTable(_userBadges)
    //                .DropDefaultProcedures(_userBadges)
    //                .DropProcedure(new SchemaProcedure("SelectUserBadgesPaged"));

    //            // Log statements to execute
    //            if (context.Logger.IsEnabled(LogLevel.Information))
    //            {
    //                context.Logger.LogInformation($"The following SQL statements will be executed...");
    //                foreach (var statement in builder.Statements)
    //                {
    //                    context.Logger.LogInformation(statement);
    //                }
    //            }

    //            // Execute statements
    //            var result = await builder.ApplySchemaAsync();
    //            if (result.Errors.Count > 0)
    //            {
    //                foreach (var error in result.Errors)
    //                {
    //                    context.Logger.LogCritical(error.Message, $"An error occurred within the UninstallingAsync method within {this.GetType().FullName}");
    //                    context.Errors.Add(error.Message, $"UninstallingAsync within {this.GetType().FullName}");
    //                }

    //            }

    //        }

    //    }

    //    public override Task UninstalledAsync(IFeatureEventContext context)
    //    {
    //        return Task.CompletedTask;
    //    }

    //    #endregion

    //    #region "Private Methods"

    //    void Configure(ISchemaBuilder builder)
    //    {

    //        builder
    //            .Configure(options =>
    //            {
    //                options.ModuleName = ModuleId;
    //                options.Version = Version;
    //                options.DropTablesBeforeCreate = true;
    //                options.DropProceduresBeforeCreate = true;
    //            });

    //    }

    //    void UserBadges(ISchemaBuilder builder)
    //    {

    //        builder
    //            .CreateTable(_userBadges)
    //            .CreateDefaultProcedures(_userBadges);
            
    //        builder.CreateProcedure(new SchemaProcedure("SelectUserBadgesPaged", StoredProcedureType.SelectPaged)
    //            .ForTable(_userBadges)
    //            .WithParameters(new List<SchemaColumn>()
    //            {
    //                new SchemaColumn()
    //                {
    //                    Name = "Keywords",
    //                    DbType = DbType.String,
    //                    Length = "255"
    //                }
    //            }));

    //    }

    //    #endregion

    //}

}
