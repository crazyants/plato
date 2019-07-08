using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class LoginViewProvider : BaseViewProvider<UserLogin>
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

        public override Task<IViewProviderResult> BuildIndexAsync(UserLogin viewModel,
            IViewProviderContext context)
        {

            var loginViewModel = new LoginViewModel()
            {
                UserName = viewModel.UserName,
                Password = viewModel.Password
            };

            return Task.FromResult(Views(
                View<LoginViewModel>("Login.Index.Header", model => loginViewModel).Zone("header"),
                View<LoginViewModel>("Login.Index.Content", model => loginViewModel).Zone("content"),
                View<LoginViewModel>("Login.Index.Sidebar", model => loginViewModel).Zone("sidebar"),
                View<LoginViewModel>("Login.Index.Footer", model => loginViewModel).Zone("footer")
            ));

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
            return await updater.TryUpdateModelAsync(new LoginViewModel
            {
                UserName = userLogin.UserName,
                Password = userLogin.Password,
                RememberMe = userLogin.RememberMe
            });
        }

        public override async Task ComposeModelAsync(UserLogin userLogin, IUpdateModel updater)
        {

            var model = new LoginViewModel()
            {
                UserName = userLogin.UserName,
                Password = userLogin.Password,
                RememberMe = userLogin.RememberMe
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                userLogin.UserName = model.UserName;
                userLogin.Password = model.Password;
                userLogin.RememberMe = model.RememberMe;
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(UserLogin viewModel, IViewProviderContext context)
        {
            return await BuildIndexAsync(viewModel, context);
        }

        #endregion

    }

}
