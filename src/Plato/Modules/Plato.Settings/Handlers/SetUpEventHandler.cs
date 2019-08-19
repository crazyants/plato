using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.Routing;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Text.Abstractions;

namespace Plato.Settings.Handlers
{
    public class SetUpEventHandler : BaseSetUpEventHandler
    {


        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IKeyGenerator _keyGenerator;

        public SetUpEventHandler(
            ISiteSettingsStore siteSettingsService, 
            IKeyGenerator keyGenerator)
        {
      
            _siteSettingsStore = siteSettingsService;
            _keyGenerator = keyGenerator;
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
                siteSettings.ApiKey = _keyGenerator.GenerateKey();
                siteSettings.HomeRoute = new HomeRoute();

                await _siteSettingsStore.SaveAsync(siteSettings);
            }
            catch (Exception ex)
            {
                reportError(ex.Message, ex.StackTrace);
            }
          
         
        }

    }

}
