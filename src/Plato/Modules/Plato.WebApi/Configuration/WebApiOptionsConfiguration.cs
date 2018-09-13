using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.WebApi.Configuration
{
    public class WebApiOptionsConfiguration : IConfigureOptions<WebApiOptions>
    {

        private readonly IContextFacade _contextFacade;
        private readonly ISiteSettingsStore _siteSettingsStore;

        public WebApiOptionsConfiguration(
            ISiteSettingsStore siteSettingsStore,
            IContextFacade contextFacade)
        {
            _siteSettingsStore = siteSettingsStore;
            _contextFacade = contextFacade;
        }

        public void Configure(WebApiOptions options)
        {

            options.Url = GetUrl().GetAwaiter().GetResult();
            options.ApiKey = GetApiKey().GetAwaiter().GetResult();

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
