using System;
using System.Threading.Tasks;
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
    public class CapturedRouterUrlHelper  : ICapturedRouterUrlHelper
    {

        private readonly IServiceCollection _applicationServices;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly ICapturedRouter _capturedRouter;

        private IUrlHelper _urlHelper;

        public CapturedRouterUrlHelper(
            ICapturedRouter capturedRouter, 
            ISiteSettingsStore siteSettingsStore,
            IUrlHelperFactory urlHelperFactory,
            IServiceCollection applicationServices)
        {
            _capturedRouter = capturedRouter;
            _siteSettingsStore = siteSettingsStore;
            _urlHelperFactory = urlHelperFactory;
            _applicationServices = applicationServices;
        }
        
        public async Task<string> GetBaseUrlAsync()
        {

            // Attempt to get baseUri from site settings
            var settings = await GetSiteSettingsAsync();
            if (!String.IsNullOrWhiteSpace(settings.BaseUrl))
            {
                // trim tailing forward slash
                var lastSlash = settings.BaseUrl.LastIndexOf('/');
                return (lastSlash > -1)
                    ? settings.BaseUrl.Substring(0, lastSlash)
                    : settings.BaseUrl;
            }

            // Fallback to baseUri provided within our configure method
            var isNullOrEmpty = String.IsNullOrEmpty(_capturedRouter.Options.BaseUrl);
            var isDefault = _capturedRouter.Options.BaseUrl?.ToLower() == "http://";
            if (isNullOrEmpty | isDefault)
            {
                throw new Exception(
                    "No BaseUrl has been captured. You must configure a BaseUrl via the Configure method before calling GetBaseUrlAsync.");
            }

            return _capturedRouter.Options.BaseUrl;

        }

        public string GetRouteUrl(string baseUri, RouteValueDictionary routeValues)
        {
            return GetRouteUrl(new Uri(baseUri), routeValues);
        }

        public string GetRouteUrl(Uri baseUri, RouteValueDictionary routeValues)
        {

            if (_capturedRouter.Options.Router == null)
            {
                throw new Exception(
                    "No router has been captured. You must configure a router via the Configure method before calling GetRouteUrl.");
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
                RouteData = new RouteData { Routers = { _capturedRouter.Options.Router } },
                ActionDescriptor = new ActionDescriptor(),
            };

            if (_urlHelper == null)
            {
                _urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);
            }

            return _urlHelper.RouteUrl(new UrlRouteContext { Values = routeValues });

        }

        #region "Private Methods"

        async Task<ISiteSettings> GetSiteSettingsAsync()
        {
            return await _siteSettingsStore.GetAsync();
        }

        #endregion

    }

}
