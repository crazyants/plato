using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Https.Models;
using Plato.Https.Stores;
using Plato.Https.ViewModels;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation;

namespace Plato.Https.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpsSettingsStore<HttpsSettings> _httpsSettingsStore;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;
        
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }


        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IAuthorizationService authorizationService,
            IHttpsSettingsStore<HttpsSettings> httpsSettingsStore,
            IAlerter alerter,
            IBreadCrumbManager breadCrumbManager)
        {
       
            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;
            _authorizationService = authorizationService;
            _httpsSettingsStore = httpsSettingsStore;

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
                ).Add(S["SSL Settings"]);
            });


            return View(await GetModel());

        }
        

        [HttpPost]
        [ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(HttpsSettingsViewModel viewModel)
        {


            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}


            if (!ModelState.IsValid)
            {
                return View(await GetModel());
            }

            var settings = new HttpsSettings()
            {
                EnforceSsl = viewModel.EnforceSsl,
                UsePermanentRedirect = viewModel.UsePermanentRedirect,
                SslPort = viewModel.SslPort

            };
            
            var result = await _httpsSettingsStore.SaveAsync(settings);
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

        private async Task<HttpsSettingsViewModel> GetModel()
        {

            var settings = await _httpsSettingsStore.GetAsync();
            if (settings != null)
            {
                return new HttpsSettingsViewModel()
                {
                    EnforceSsl = settings.EnforceSsl,
                    UsePermanentRedirect = settings.UsePermanentRedirect,
                    SslPort = settings.SslPort

                };
            }

            return new HttpsSettingsViewModel()
            {
                EnforceSsl = false,
                UsePermanentRedirect = false,
                SslPort = 443

            };


        }


        #endregion


    }
}
