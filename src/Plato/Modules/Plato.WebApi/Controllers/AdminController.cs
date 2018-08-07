using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.WebApi.ViewModels;

namespace Plato.WebApi.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IAuthorizationService _authorizationService;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IAuthorizationService authorizationService,
            IAlerter alerter, ISiteSettingsStore siteSettingsStore,
            IBreadCrumbManager breadCrumbManager)
        {
            _alerter = alerter;
            _siteSettingsStore = siteSettingsStore;
            _breadCrumbManager = breadCrumbManager;
            _authorizationService = authorizationService;

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
                ).Add(S["Web Api Settings"]);
            });


            return View(await GetModel());

        }

        public async Task<IActionResult> CreateApiKey()
        {

            var settings = await _siteSettingsStore.GetAsync();
            if (settings == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            settings.ApiKey = System.Guid.NewGuid().ToString();
         
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
        
        [HttpPost]
        [ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(EditSettingsViewModel viewModel)
        {


            // TODO: Impelement security
            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}


            if (!ModelState.IsValid)
            {
                return View(await GetModel());
            }

            // Update existing settings
            var settings = await _siteSettingsStore.GetAsync();
            if (settings != null)
            {
                settings.ApiKey = viewModel.ApiKey;
            }
            else
            {
                // Create new settings
                settings = new SiteSettings()
                {
                    ApiKey = viewModel.ApiKey
                };
            }
            
            // Update settings
            var result = await _siteSettingsStore.SaveAsync(settings);
            if (result != null)
            {
                _alerter.Success(T["Settings Updated Successfully!"]);
            }
            else
            {
                _alerter.Danger(T["A problem occurred updating the settings. Please try again!"]);
            }
            
            return RedirectToAction(nameof(Index));
            
        }
        
        #endregion

        #region "Private Methods"

        private async Task<EditSettingsViewModel> GetModel()
        {

            var settings = await _siteSettingsStore.GetAsync();

            if (settings != null)
            {
                return new EditSettingsViewModel()
                {
                    ApiKey = settings.ApiKey

                };
            }
            
            // return default settings
            return new EditSettingsViewModel()
            {
                ApiKey = System.Guid.NewGuid().ToString()
            };

        }


        #endregion


    }
}
