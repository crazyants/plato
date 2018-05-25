using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Abstractions.SetUp;
using Plato.Abstractions.Stores;
using Plato.Data.Abstractions.Schemas;
using Plato.Models.Settings;

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

                var result = builder.Apply();
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

            var siteSettings = await _siteSettingsService.GetAsync();
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
    }

}
