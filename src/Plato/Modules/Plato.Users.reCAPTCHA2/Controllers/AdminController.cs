using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Users.reCAPTCHA2.Models;
using Plato.Users.reCAPTCHA2.Stores;
using Plato.Users.reCAPTCHA2.ViewModels;

namespace Plato.Users.reCAPTCHA2.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {

        private readonly IAuthorizationService _authorizationService;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IViewProviderManager<ReCaptchaSettings> _viewProvider;
        private readonly IReCaptchaSettingsStore<ReCaptchaSettings> _recaptchaSettingsStore;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IAuthorizationService authorizationService,
            IAlerter alerter, 
            IBreadCrumbManager breadCrumbManager,
            IViewProviderManager<ReCaptchaSettings> viewProvider,
            IReCaptchaSettingsStore<ReCaptchaSettings> recaptchaSettingsStore)
        {
            _authorizationService = authorizationService;
            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;
            _recaptchaSettingsStore = recaptchaSettingsStore;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<IActionResult> Index()
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Settings"], channels => channels
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["reCAPTCHA2 Settings"]);
            });

            // Get reCAPTCHA settings
            var settings = await _recaptchaSettingsStore.GetAsync();

            // Build view
            var result = await _viewProvider.ProvideEditAsync(new ReCaptchaSettings()
            {
                SiteKey = settings?.SiteKey ?? "",
                Secret = settings?.Secret ?? ""
            }, this);

            //Return view
            return View(result);

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(ReCaptchaSettingsViewModel viewModel)
        {

            // Execute view providers ProvideUpdateAsync method
            await _viewProvider.ProvideUpdateAsync(new ReCaptchaSettings()
            {
                SiteKey = viewModel.SiteKey,
                Secret = viewModel.Secret
            }, this);

            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);

            return RedirectToAction(nameof(Index));

        }

    }

}
