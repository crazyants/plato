using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Abstractions.SetUp;

namespace Plato.Settings.Services
{
    public class SetUpEventHandler : ISetUpEventHandler
    {
        private readonly ISettingsService _setupService;

        public SetUpEventHandler(ISettingsService setupService)
        {
            _setupService = setupService;
        }

        public async Task SetUp(
            SetUpContext context,
            Action<string, string> reportError
        )
        {

            // Updating site settings
            var siteSettings = await _setupService.GetAsync();
            siteSettings.SiteName = context.SiteName;
            siteSettings.SuperUser = context.AdminUsername;
            siteSettings.HomeRoute = new RouteValueDictionary();
            await _setupService.SaveAsync(siteSettings);

            // TODO: Add Encryption Settings in
        }
    }

}
