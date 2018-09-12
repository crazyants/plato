using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Internal.Shell
{
    public class ContextFacade : IContextFacade
    {

        public const string DefaultCulture = "en-US";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IUrlHelperFactory _urlHelperFactory;

        private IUrlHelper _urlHelper;

        public ContextFacade(
            IHttpContextAccessor httpContextAccessor,
            IPlatoUserStore<User> platoUserStore,
            IShellDescriptorManager shellDescriptorManager,
            IActionContextAccessor actionContextAccessor,
            ISiteSettingsStore siteSettingsStore,
            IUrlHelperFactory urlHelperFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _platoUserStore = platoUserStore;
            _shellDescriptorManager = shellDescriptorManager;
            _actionContextAccessor = actionContextAccessor;
            _siteSettingsStore = siteSettingsStore;
            _urlHelperFactory = urlHelperFactory;
        }

        public async Task<User> GetAuthenticatedUserAsync()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var identity = user?.Identity;
            if ((identity != null) && (identity.IsAuthenticated))
            {
                return await _platoUserStore.GetByUserNameAsync(identity.Name);
            }

            return null;
        }

        public async Task<ShellModule> GetFeatureByAreaAsync()
        {
            // Current area name
            var areaName = (string) _actionContextAccessor.ActionContext
                .RouteData.Values["area"];
            return await GetFeatureByModuleIdAsync(areaName);
        }

        public async Task<ShellModule> GetFeatureByModuleIdAsync(string areaName)
        {

            // Get module from descriptor matching areaName
            var descriptor = await _shellDescriptorManager.GetEnabledDescriptorAsync();
            if (descriptor != null)
            {
                return descriptor.Modules?
                           .FirstOrDefault(m => m.ModuleId == areaName)
                       ?? null;
            }

            throw new Exception($"There was a problem obtaining the feature for the area name {areaName}.");

        }


        public async Task<ISiteSettings> GetSiteSettingsAsync()
        {
            return await _siteSettingsStore.GetAsync();
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

            var request = _httpContextAccessor.HttpContext.Request;
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

        public string GetCurrentCulture()
        {
            return GetCurrentCultureAsync()
                .GetAwaiter()
                .GetResult();
        }

    }

}
