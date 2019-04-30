using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Settings.Models;
using Plato.Settings.ViewModels;
using Plato.Internal.Security.Abstractions;

namespace Plato.Settings.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IAuthorizationService _authorizationService;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IViewProviderManager<SettingsIndex> _viewProvider;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IViewProviderManager<SettingsIndex> viewProvider,
            IAuthorizationService authorizationService,
            ISiteSettingsStore siteSettingsStore,
            IBreadCrumbManager breadCrumbManager,
            IAlerter alerter)
        {

            _authorizationService = authorizationService;
            _siteSettingsStore = siteSettingsStore;
            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index()
        {
            
            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageGeneralSettings))
            {
                return Unauthorized();
            }

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Settings"], channels => channels
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["General"]);
            });
            
            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(new SettingsIndex(), this));

        }

        [HttpPost, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(SiteSettingsViewModel viewModel)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageGeneralSettings))
            {
                return Unauthorized();
            }
            
            // Execute view providers ProvideUpdateAsync method
            await _viewProvider.ProvideUpdateAsync(new SettingsIndex(), this);

            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);

            // Redirect
            return RedirectToAction(nameof(Index));

        }
        
        public async Task<IActionResult> CreateApiKey()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageGeneralSettings))
            {
                return Unauthorized();
            }
            
            var settings = await _siteSettingsStore.GetAsync();
            if (settings == null)
            {
                return RedirectToAction(nameof(Index));
            }

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
        
        #endregion

    }
}
