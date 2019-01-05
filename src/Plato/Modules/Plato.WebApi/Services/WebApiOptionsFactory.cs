using System;
using System.Threading.Tasks;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.WebApi.Models;

namespace Plato.WebApi.Services
{
    
    public class WebApiOptionsFactory : IWebApiOptionsFactory
    {

        private readonly IContextFacade _contextFacade;
        private readonly ISiteSettingsStore _siteSettingsStore;

        public WebApiOptionsFactory(
            IContextFacade contextFacade,
            ISiteSettingsStore siteSettingsStore)
        {
            _contextFacade = contextFacade;
            _siteSettingsStore = siteSettingsStore;
        }

        public async Task<WebApiOptions> GetSettingsAsync()
        {
            return new WebApiOptions()
            {
                Url = await GetUrl(),
                ApiKey = await GetApiKey()
            };
        }

        async Task<string> GetUrl()
        {
            return await _contextFacade.GetBaseUrlAsync();
        }

        async Task<string> GetApiKey()
        {

            var settings = await _contextFacade.GetSiteSettingsAsync();

            if (settings == null)
            {
                return string.Empty;
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return settings.ApiKey;
            }

            if (String.IsNullOrWhiteSpace(user.ApiKey))
            {
                return settings.ApiKey;
            }

            return $"{settings.ApiKey}:{user.ApiKey}";

        }

    }

}
