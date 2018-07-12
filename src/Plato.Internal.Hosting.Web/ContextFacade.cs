using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Internal.Hosting.Web
{
    public class ContextFacade : IContextFacade
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly ISiteSettingsStore _siteSettingsStore; 

        public ContextFacade(
            IHttpContextAccessor httpContextAccessor,
            IPlatoUserStore<User> platoUserStore,
            IShellDescriptorManager shellDescriptorManager, IActionContextAccessor actionContextAccessor, ISiteSettingsStore siteSettingsStore)
        {
            _httpContextAccessor = httpContextAccessor;
            _platoUserStore = platoUserStore;
            _shellDescriptorManager = shellDescriptorManager;
            _actionContextAccessor = actionContextAccessor;
            _siteSettingsStore = siteSettingsStore;
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
            var areaName =(string) _actionContextAccessor.ActionContext
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

        public async Task<string> GetBaseUrl()
        {

            var settings = await GetSiteSettingsAsync();
            if (!String.IsNullOrWhiteSpace(settings.BaseUrl))
            {
                if (settings.BaseUrl.EndsWith("/"))
                {
                    return settings.BaseUrl;
                }
                return settings.BaseUrl + "/";
            }
            
            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{request.PathBase}/";
            
        }

    }

}
