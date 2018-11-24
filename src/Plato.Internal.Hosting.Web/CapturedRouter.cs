using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Internal.Hosting.Web
{

    /// <summary>
    /// Allows the use of IUrlHelperFactory outside of an ASP.NET core application context.
    /// IUrlHelperFactory requires an ActionContext which is not available within background tasks.
    /// </summary>
    public class CapturedRouter : ICapturedRouter
    {

        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IServiceCollection _applicationServices;
        private readonly ISiteSettingsStore _siteSettingsStore;

        private IUrlHelper _urlHelper;
        private CapturedRouterOptions _options;

        public CapturedRouter(
            IUrlHelperFactory urlHelperFactory,
            IServiceCollection applicationServices,
            ISiteSettingsStore siteSettingsStore)
        {
            _urlHelperFactory = urlHelperFactory;
            _applicationServices = applicationServices;
            _siteSettingsStore = siteSettingsStore;
        }

        #region "Implementation"

        public ICapturedRouter Configure(Action<CapturedRouterOptions> configure)
        {
            var options = new CapturedRouterOptions();
            configure(options);
            _options = options;
            return this;
        }
        
        public async Task<string> GetBaseUrlAsync()
        {

            var settings = await GetSiteSettingsAsync();
            if (!String.IsNullOrWhiteSpace(settings.BaseUrl))
            {
                // trim tailing forward slash
                var lastSlash = settings.BaseUrl.LastIndexOf('/');
                return (lastSlash > -1)
                    ? settings.BaseUrl.Substring(0, lastSlash)
                    : settings.BaseUrl;
            }
            
            return _options.BaseUrl;

        }

        public string GetRouteUrl(string baseUri, RouteValueDictionary routeValues)
        {
            return GetRouteUrl(new Uri(baseUri), routeValues);
        }

        public string GetRouteUrl(Uri baseUri, RouteValueDictionary routeValues)
        {

            if (_options.Router == null)
            {
                throw new Exception(
                    $"No router has been captured. You must specify a router to use before calling GetRouteUrl. Call UseRouter first.");
            }
            
            var httpContext = new DefaultHttpContext()
            {
                RequestServices = _applicationServices.BuildServiceProvider(),
                Request =
                {
                    Scheme = baseUri.Scheme,
                    Host = HostString.FromUriComponent(baseUri),
                    PathBase = PathString.FromUriComponent(baseUri),
                },
            };
            
            var actionContext = new ActionContext
            {
                HttpContext = httpContext,
                RouteData = new RouteData { Routers = { _options.Router } },
                ActionDescriptor = new ActionDescriptor(),
            };

            if (_urlHelper == null)
            {
                _urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);
            }

            return _urlHelper.RouteUrl(new UrlRouteContext { Values = routeValues });

        }

        #endregion

        #region "Private Methods"

        async Task<ISiteSettings> GetSiteSettingsAsync()
        {
            return await _siteSettingsStore.GetAsync();
        }

        #endregion

    }




}
