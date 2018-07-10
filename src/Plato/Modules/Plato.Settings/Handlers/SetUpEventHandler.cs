using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Settings.Handlers
{
    public class SetUpEventHandler : BaseSetUpEventHandler
    {

        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISiteSettingsStore _siteSettingsStore;
    
        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder,
            ISiteSettingsStore siteSettingsService)
        {
            _schemaBuilder = schemaBuilder;
            _siteSettingsStore = siteSettingsService;
        }

        public override async Task SetUp(
            SetUpContext context,
            Action<string, string> reportError)
        {
            
            // --------------------------
            // Add default settings to dictionary store
            // --------------------------

            try
            {

                var siteSettings = await _siteSettingsStore.GetAsync() ?? new SiteSettings();
                siteSettings.SiteName = context.SiteName;
                siteSettings.SuperUser = context.AdminUsername;
           
                // add default route for set-up site
                siteSettings.HomeRoute = new RouteValueDictionary()
                {
                    { "Area", "Plato.Users" },
                    { "Controller", "Account" },
                    { "Action", "Login" }

                };

                await _siteSettingsStore.SaveAsync(siteSettings);
            }
            catch (Exception ex)
            {
                reportError(ex.Message, ex.StackTrace);
            }
          
         
        }

    }

}
