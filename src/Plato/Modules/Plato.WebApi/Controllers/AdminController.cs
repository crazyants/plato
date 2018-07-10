using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
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

        public IHtmlLocalizer T { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> localizer,
            IAuthorizationService authorizationService,
            IAlerter alerter, ISiteSettingsStore siteSettingsStore)
        {
            _alerter = alerter;
            _siteSettingsStore = siteSettingsStore;
            _authorizationService = authorizationService;
            T = localizer;
        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index()
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}
            
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


            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}


            if (!ModelState.IsValid)
            {
                return View(await GetModel());
            }
            
            var settings = new SiteSettings()
            {
                ApiKey = viewModel.ApiKey
            };
            
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
