using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.WebApi.Services
{

    public class WebApisettings
    {
        public string Url { get; set; }

        public string ApiKey { get; set; }

    }

    public interface IWebApiSettingsFactory
    {
        Task<WebApisettings> GetSettingsAsync();
    }

    public class WebApiSettingsFactory : IWebApiSettingsFactory
    {

        private readonly IContextFacade _contextFacade;
        private readonly ISiteSettingsStore _siteSettingsStore;

        public WebApiSettingsFactory(
            IContextFacade contextFacade,
            ISiteSettingsStore siteSettingsStore)
        {
            _contextFacade = contextFacade;
            _siteSettingsStore = siteSettingsStore;
        }

        public async Task<WebApisettings> GetSettingsAsync()
        {
            return new WebApisettings()
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
