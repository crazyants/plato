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
                ).Add(S["General"]);
            });
            
            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(new SettingsIndex(), this));

        }

        [HttpPost, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(SiteSettingsViewModel viewModel)
        {

            // Execute view providers ProvideUpdateAsync method
            await _viewProvider.ProvideUpdateAsync(new SettingsIndex(), this);

            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);

            // Redirect
            return RedirectToAction(nameof(Index));

            //// TODO: Implement security
            ////if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            ////{
            ////    return Unauthorized();
            ////}

            //if (!ModelState.IsValid)
            //{
            //    foreach (var modelState in ViewData.ModelState.Values)
            //    {
            //        foreach (var error in modelState.Errors)
            //        {
            //            _alerter.Danger(T[error.ErrorMessage]);
            //        }
            //    }
            //    return View();
            //}

            //// Update existing settings
            //var settings = await _siteSettingsStore.GetAsync();
            //if (settings != null)
            //{
            //    settings.SiteName = viewModel.SiteName;
            //    settings.TimeZone = viewModel.TimeZone;
            //    settings.DateTimeFormat = viewModel.DateTimeFormat;
            //    settings.Culture = viewModel.Culture;
            //    settings.Theme = viewModel.Theme;
            //}
            //else
            //{
            //    // Create new settings
            //    settings = new SiteSettings()
            //    {
            //        SiteName = viewModel.SiteName,
            //        TimeZone = viewModel.TimeZone,
            //        DateTimeFormat = viewModel.DateTimeFormat,
            //        Culture = viewModel.Culture,
            //        Theme = viewModel.Theme
            //    };
            //}
        
            //// Update settings
            //var result = await _siteSettingsStore.SaveAsync(settings);
            //if (result != null)
            //{

            //    // Recycle shell context to ensure changes take effect
            //    _platoHost.RecycleShellContext(_shellSettings);

            //    _alerter.Success(T["Settings Updated Successfully!"]);
            //}
            //else
            //{
            //    _alerter.Danger(T["A problem occurred updating the settings. Please try again!"]);
            //}
            
            //return RedirectToAction(nameof(Index));
            
        }
        
        public async Task<IActionResult> CreateApiKey()
        {

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
