using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Users.ViewModels;
using Plato.Users.Google.reCAPTCHA3.ViewModels;

namespace Plato.Users.Google.reCAPTCHA3.ViewProviders
{
    public class RegisterViewProvider : BaseViewProvider<RegisterViewModel>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(RegisterViewModel viewModel, IViewProviderContext context)
        {

            var recaptchaViewMddel = new ReCaptchaViewModel();

            return Task.FromResult(Views(
                 View<ReCaptchaViewModel>("Register.Google.reCAPTCHA3", model => recaptchaViewMddel).Zone("content").Order(int.MaxValue)
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
