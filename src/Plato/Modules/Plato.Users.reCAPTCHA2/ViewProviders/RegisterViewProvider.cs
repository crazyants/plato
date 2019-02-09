using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Users.reCAPTCHA2.ViewModels;
using Plato.Users.ViewModels;

namespace Plato.Users.reCAPTCHA2.ViewProviders
{
    public class RegisterViewProvider : BaseViewProvider<RegisterViewModel>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(RegisterViewModel viewModel, IViewProviderContext context)
        {

            var recaptchaViewModel = new ReCaptchaViewModel();

            return Task.FromResult(Views(
                 View<ReCaptchaViewModel>("Register.Google.reCAPTCHA2", model => recaptchaViewModel).Zone("content").Order(int.MaxValue)
             ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(RegisterViewModel viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));

        }
        
        public override Task<IViewProviderResult> BuildEditAsync(RegisterViewModel viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(RegisterViewModel viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));

        }
    }
}
