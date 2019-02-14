using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Users.reCAPTCHA2.Models;
using Plato.Users.reCAPTCHA2.Services;
using Plato.Users.reCAPTCHA2.Stores;
using Plato.Users.reCAPTCHA2.ViewModels;

namespace Plato.Users.reCAPTCHA2.ViewProviders
{
    public class LoginViewProvider : BaseViewProvider<UserLogin>
    {

        private readonly IReCaptchaService _recaptchaService;
        private readonly IReCaptchaSettingsStore<ReCaptchaSettings> _recaptchaSettingsStore;
        private readonly HttpRequest _request;

        public LoginViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IReCaptchaService recaptchaService,
            IReCaptchaSettingsStore<ReCaptchaSettings> recaptchaSettingsStore)
        {
            _recaptchaService = recaptchaService;
            _recaptchaSettingsStore = recaptchaSettingsStore;
            _request = httpContextAccessor.HttpContext.Request;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(UserLogin viewModel,
            IViewProviderContext context)
        {

            // Get settings
            var settings = await _recaptchaSettingsStore.GetAsync();

            // Build view model
            var recaptchaViewModel = new ReCaptchaViewModel()
            {
                SiteKey = settings?.SiteKey ?? "",
                Secret = settings?.Secret ?? ""
            };

            // Build view
            return Views(
                View<ReCaptchaViewModel>("Google.reCAPTCHA2", model => recaptchaViewModel).Zone("content")
                    .Order(int.MaxValue)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(UserLogin viewModel,
            IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(UserLogin viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<bool> ValidateModelAsync(UserLogin userLogin, IUpdateModel updater)
        {

            // Get settings
            var settings = await _recaptchaSettingsStore.GetAsync();

            // If we don't have any settings we can't perform the checks 
            // Ensure we return true to still allow the user to login

            if (settings == null)
            {
                return true;
            }

            if (string.IsNullOrEmpty(settings.SiteKey))
            {
                return true;
            }

            if (string.IsNullOrEmpty(settings.Secret))
            {
                return true;
            }

            // Get posted response
            var recaptchaResponse = _request.Form["g-recaptcha-response"];

            // No valid posted - user has probably not ticked check box
            if (String.IsNullOrEmpty(recaptchaResponse))
            {
                updater.ModelState.AddModelError(string.Empty, "You must indicate your not a robot!");
                return false;
            }

            // Validate posted recaptcha response
            var response = await _recaptchaService.Validate(recaptchaResponse);

            // No response received
            if (response == null)
            {
                updater.ModelState.AddModelError(string.Empty,
                    "A problem occurred communicating with the Google reCAPTCHA service. Please try again. If the problem persists please contact us.");
                return false;
            }

            // Configuration issues?
            if (response.ErrorCodes.Count > 0)
            {
                updater.ModelState.AddModelError(string.Empty,
                    "A configuration error with Google reCAPTCHA has occurred. Response from Google: " +
                    String.Join(Environment.NewLine, response.ErrorCodes.Select(c => c).ToArray()));
                return false;
            }

            // Response failed
            if (!response.Succeeded)
            {
                updater.ModelState.AddModelError(string.Empty,
                    "The Google reCAPTCHA service could not validate you are human. If this is incorrect please contact us.");
                return false;
            }


            return true;

        }


        public override async Task<IViewProviderResult> BuildUpdateAsync(UserLogin viewModel,
            IViewProviderContext context)
        {
            return await BuildIndexAsync(viewModel, context);
        }
        
    }

}
