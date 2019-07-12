using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Text.Abstractions;
using Plato.Internal.Text.Abstractions.Diff.Models;
using Plato.WebApi.Models;
using Plato.WebApi.ViewModels;

namespace Plato.WebApi.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IViewProviderManager<WebApiSettings> _viewProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly UserManager<User> _userManager;
        private readonly IContextFacade _contextFacade;
        private readonly IKeyGenerator _keyGenerator;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IViewProviderManager<WebApiSettings> viewProvider,
            IAuthorizationService authorizationService,
            ISiteSettingsStore siteSettingsStore,
            IBreadCrumbManager breadCrumbManager,
            UserManager<User> userManager,
            IContextFacade contextFacade,
            IKeyGenerator keyGenerator,
            IAlerter alerter)
        {
        
            _authorizationService = authorizationService;
            _siteSettingsStore = siteSettingsStore;
            _breadCrumbManager = breadCrumbManager;
            _contextFacade = contextFacade;
            _viewProvider = viewProvider;
            _keyGenerator = keyGenerator;
            _userManager = userManager;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"
        
        // ------------
        // Settings
        // ------------
        
        public async Task<IActionResult> Index()
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Settings"], channels => channels
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["Web Api"]);
            });

            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(new WebApiSettings(), this));

        }
 
        [HttpPost]
        [ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(WebApiSettingsViewModel viewModel)
        {

            // Execute view providers ProvideUpdateAsync method
            await _viewProvider.ProvideUpdateAsync(new WebApiSettings(), this);

            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);

            return RedirectToAction(nameof(Index));

        }
        
        // ------------
        // Reset App Api Key
        // ------------
        
        public async Task<IActionResult> ResetApiKey()
        {

            var settings = await _siteSettingsStore.GetAsync();
            if (settings == null)
            {
                return RedirectToAction(nameof(Index));
            }

            settings.ApiKey = _keyGenerator.GenerateKey();

            var result = await _siteSettingsStore.SaveAsync(settings);
            if (result != null)
            {
                _alerter.Success(T["Key Updated Successfully!"]);
            }
            else
            {
                _alerter.Danger(T["A problem occurred updating the settings. Please try again!"]);
            }

            return RedirectToAction(nameof(Index));
        }

        // ------------
        // Reset User Api Key
        // ------------

        public async Task<IActionResult> ResetUserApiKey(string id)
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.ResetWebApiKeys))
            {
                return Unauthorized();
            }

            // Get user we are editing
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Generate new API Key
            currentUser.ApiKey = _keyGenerator.GenerateKey(o =>
            {
                o.UniqueIdentifier = currentUser.Id.ToString();
            });

            // Update user
            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                _alerter.Success(T["Key Reset Successfully!"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }

            // Could not locate offset, fallback by redirecting to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Users",
                ["controller"] = "Admin",
                ["action"] = "Edit",
                ["id"] = currentUser.Id.ToString()
            }));

        }
        
        #endregion

        #region "Private Methods"

        private async Task<WebApiSettingsViewModel> GetModel()
        {

            var settings = await _siteSettingsStore.GetAsync();

            if (settings != null)
            {
                return new WebApiSettingsViewModel()
                {
                    ApiKey = settings.ApiKey

                };
            }
            
            // return default settings
            return new WebApiSettingsViewModel()
            {
                ApiKey = _keyGenerator.GenerateKey()
            };

        }
        
        #endregion
        
    }

}
