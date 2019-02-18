using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Internal.Hosting.Web
{
    public class ContextFacade : IContextFacade
    {

        public const string DefaultCulture = "en-US";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IUrlHelperFactory _urlHelperFactory;
        
        private IUrlHelper _urlHelper;

        public ContextFacade(
            IHttpContextAccessor httpContextAccessor,
            IPlatoUserStore<User> platoUserStore,
            IActionContextAccessor actionContextAccessor,
            ISiteSettingsStore siteSettingsStore,
            IUrlHelperFactory urlHelperFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _platoUserStore = platoUserStore;
            _actionContextAccessor = actionContextAccessor;
            _siteSettingsStore = siteSettingsStore;
            _urlHelperFactory = urlHelperFactory;
        }

        public async Task<User> GetAuthenticatedUserAsync()
        {
        
            var identity = _httpContextAccessor.HttpContext.User?.Identity;
            if ((identity != null) && (identity.IsAuthenticated))
            {
                return await _platoUserStore.GetByUserNameAsync(identity.Name);
            }

            return null;
        }
        
        public async Task<ISiteSettings> GetSiteSettingsAsync()
        {
            return await _siteSettingsStore.GetAsync();
        }

        public async Task<string> GetBaseUrlAsync()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            return await GetBaseUrlAsync(request);
        }

        public async Task<string> GetBaseUrlAsync(HttpRequest request)
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

            return $"{request.Scheme}://{request.Host}{request.PathBase}";
        }

        public string GetRouteUrl(RouteValueDictionary routeValues)
        {

            if (_urlHelper == null)
            {
                _urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            }

            return _urlHelper.RouteUrl(new UrlRouteContext {Values = routeValues});
        }

        public async Task<string> GetCurrentCultureAsync()
        {

            // Get users culture
            var user = await GetAuthenticatedUserAsync();
            if (user != null)
            {
                if (!String.IsNullOrEmpty(user.Culture))
                {
                    return user.Culture;
                }
                
            }

            // Get application culture
            var settings = await GetSiteSettingsAsync();
            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.Culture))
                {
                    return settings.Culture;
                }
            }
       
            // Return en-US default culture
            return DefaultCulture;

        }

        //public string GetCurrentCulture()
        //{
        //    return GetCurrentCultureAsync()
        //        .GetAwaiter()
        //        .GetResult();
        //}

    }

}
