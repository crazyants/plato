using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Models.Settings;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Settings.Services
{
    public class SetUpEventHandler : ISetUpEventHandler
    {

        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISiteSettingsStore _siteSettingsService;
    
        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder,
            ISiteSettingsStore siteSettingsService)
        {
            _schemaBuilder = schemaBuilder;
            _siteSettingsService = siteSettingsService;
        }

        public async Task SetUp(
            SetUpContext context,
            Action<string, string> reportError
        )
        {
            
            // --------------------------
            // Build schema
            // --------------------------

            var table = new SchemaTable()
            {
                Name = "Settings",
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
                        Name = "[Key]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Value]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTime2
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTime2
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    }
                }
            };
            
            using (var builder = _schemaBuilder)
            {
                builder
                    .Configure(options =>
                    {
                        options.ModuleName = "Plato.Settings";
                        options.Version = "1.0.0";
                    })
                    .CreateTable(table)
                    .CreateDefaultProcedures(table)
                    .CreateProcedure(new SchemaProcedure("SelectSettingByKey", StoredProcedureType.SelectByKey)
                        .ForTable(table).WithParameter(new SchemaColumn() {Name = "[Key]", DbType = DbType.Int32}));

                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        reportError(error.Message, error.StackTrace);
                    }
                 
                }
            }

            // --------------------------
            // Add default data
            // --------------------------

            try
            {

                var siteSettings = await _siteSettingsService.GetAsync() ?? new SiteSettings();
                siteSettings.SiteName = context.SiteName;
                siteSettings.SuperUser = context.AdminUsername;

                // add default route for set-up site
                siteSettings.HomeRoute = new RouteValueDictionary()
                {
                    { "Area", "Plato.Users" },
                    { "Controller", "Account" },
                    { "Action", "Login" }

                };

                await _siteSettingsService.SaveAsync(siteSettings);
            }
            catch (Exception ex)
            {
                reportError(ex.Message, ex.StackTrace);
            }
          
        
         
        }
    }

}
