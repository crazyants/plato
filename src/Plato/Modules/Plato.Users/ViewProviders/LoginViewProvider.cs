using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class LoginViewProvider : BaseViewProvider<LoginViewModel>
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginViewProvider> _logger;
        private readonly IOptions<IdentityOptions> _identityOptions;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public LoginViewProvider(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            SignInManager<User> signInManager,
            ILogger<LoginViewProvider> logger,
            IOptions<IdentityOptions> identityOptions,
            UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _identityOptions = identityOptions;
            _logger = logger;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(LoginViewModel viewModel,
            IViewProviderContext context)
        {

            return Task.FromResult(Views(
                View<LoginViewModel>("Login.Index.Header", model => viewModel).Zone("header"),
                View<LoginViewModel>("Login.Index.Content", model => viewModel).Zone("content"),
                View<LoginViewModel>("Login.Index.Sidebar", model => viewModel).Zone("sidebar"),
                View<LoginViewModel>("Login.Index.Footer", model => viewModel).Zone("footer")
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(LoginViewModel viewModel,
            IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(LoginViewModel viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(LoginViewModel viewModel, IViewProviderContext context)
        {

            // Validate view model
            if (await ValidateModelAsync(viewModel, context.Updater))
            {

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(
                    viewModel.UserName,
                    viewModel.Password,
                    viewModel.RememberMe,
                    lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return await BuildIndexAsync(viewModel, context);
                }

                // If we reach this point authentication failed for some reason

                if (result.RequiresTwoFactor)
                {
                    //return RedirectToAction(nameof(LoginWith2fa), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    context.Controller.ModelState.AddModelError(string.Empty,
                        "Account Required Two Factor Authentication.");
                    return await BuildIndexAsync(viewModel, context);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    context.Controller.ModelState.AddModelError(string.Empty, "Account Locked out.");
                    return await BuildIndexAsync(viewModel, context);
                }

                // Inform the user the account requires confirmation
                if (_identityOptions.Value.SignIn.RequireConfirmedEmail)
                {
                    var user = await _userManager.FindByNameAsync(viewModel.UserName);
                    if (user != null)
                    {
                        var validPassword = await _userManager.CheckPasswordAsync(user, viewModel.Password);
                        if (validPassword)
                        {
                            // Valid credentials entered
                            context.Controller.ModelState.AddModelError(string.Empty,
                                "Before you can login you must first confirm your email address. Use the \"Confirm your email address\" link below to resend your account confirmation email.");
                            return await BuildIndexAsync(viewModel, context);
                        }
                    }
                }

                // Invalid login credentials
                context.Controller.ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            }

            return await BuildIndexAsync(viewModel, context);
            
        }

        #endregion

    }

}
