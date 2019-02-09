using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Layout.ViewProviders;
using Plato.Users.reCAPTCHA2.Models;
using Plato.Users.reCAPTCHA2.Services;
using Plato.Users.reCAPTCHA2.Stores;
using Plato.Users.reCAPTCHA2.ViewModels;
using Plato.Users.ViewModels;

namespace Plato.Users.reCAPTCHA2.ViewProviders
{
    public class RegisterViewProvider : BaseViewProvider<RegisterViewModel>
    {

        private readonly IReCaptchaService _recaptchaService;
        private readonly IReCaptchaSettingsStore<ReCaptchaSettings> _recaptchaSettingsStore;
        private readonly HttpRequest _request;

        public RegisterViewProvider(
            IReCaptchaService recaptchaService,
            IHttpContextAccessor httpContextAccessor,
            IReCaptchaSettingsStore<ReCaptchaSettings> recaptchaSettingsStore)
        {
            _recaptchaService = recaptchaService;
            _request = httpContextAccessor.HttpContext.Request;
            _recaptchaSettingsStore = recaptchaSettingsStore;
        }
        
        public override async Task<IViewProviderResult> BuildIndexAsync(RegisterViewModel viewModel, IViewProviderContext context)
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
                View<ReCaptchaViewModel>("Register.Google.reCAPTCHA2", model => recaptchaViewModel).Zone("content")
                    .Order(int.MaxValue)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(RegisterViewModel viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(RegisterViewModel viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(RegisterViewModel viewModel, IViewProviderContext context)
        {

            if (await ValidateModelAsync(viewModel, context.Updater))
            {

                var recaptchaResponse = _request.Form["g-recaptcha-response"];

                // No valid posted - user has probably not ticked check box
                if (String.IsNullOrEmpty(recaptchaResponse))
                {
                    context.Updater.ModelState.AddModelError(string.Empty, "You must indicate your not a robot!");
                    return await BuildIndexAsync(viewModel, context);
                }

                // Validate posted recaptcha response
                var response = await _recaptchaService.Validate(recaptchaResponse);

                // No response received
                if (response == null)
                {
                    context.Controller.ModelState.AddModelError(string.Empty,
                        "A problem occurred communicating with the Google reCAPTCHA service. Please try again. If the problem persists please contact us.");
                    return await BuildIndexAsync(viewModel, context);
                }

                // Configuration issues?
                if (response.ErrorCodes.Count > 0)
                {
                    context.Controller.ModelState.AddModelError(string.Empty,
                        "A configuration error with Google reCAPTCHA has occurred. Response from Google: " +
                        String.Join(Environment.NewLine, response.ErrorCodes.Select(c => c).ToArray()));
                    return await BuildIndexAsync(viewModel, context);
                }

                // Response failed
                if (!response.Succeeded)
                {
                    context.Controller.ModelState.AddModelError(string.Empty,
                        "The Google reCAPTCHA service could not validate you are human. If this is incorrect please contact us.");
                    return await BuildIndexAsync(viewModel, context);
                }

            }

            return await BuildIndexAsync(viewModel, context);

        }

    }

}
