using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Text.Abstractions;
using Plato.WebApi.Models;
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
        private readonly IViewProviderManager<WebApiSettings> _viewProvider;
        private readonly IKeyGenerator _keyGenerator;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IAuthorizationService authorizationService,
            IAlerter alerter, ISiteSettingsStore siteSettingsStore,
            IBreadCrumbManager breadCrumbManager,
            IViewProviderManager<WebApiSettings> viewProvider, 
            IKeyGenerator keyGenerator)
        {
            _alerter = alerter;
            _siteSettingsStore = siteSettingsStore;
            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;
            _keyGenerator = keyGenerator;
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

            // Build view
            var result = await _viewProvider.ProvideEditAsync(new WebApiSettings(), this);

            // Return view
            return View(result);
            
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

        public async Task<IActionResult> CreateApiKey()
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
