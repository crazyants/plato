using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Users.reCAPTCHA2.Models;
using Plato.Users.reCAPTCHA2.ViewModels;

namespace Plato.Users.reCAPTCHA2.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<ReCaptchaSettings>
    {

        public override Task<IViewProviderResult> BuildDisplayAsync(ReCaptchaSettings viewModel, IViewProviderContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<IViewProviderResult> BuildIndexAsync(ReCaptchaSettings viewModel, IViewProviderContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<IViewProviderResult> BuildEditAsync(ReCaptchaSettings viewModel, IViewProviderContext context)
        {
            
            var recaptchaSettingsViewModel = new ReCaptchaSettingsViewModel()
            {
                SiteKey = viewModel.SiteKey,
                Secret = viewModel.Secret
            };

            return Task.FromResult(Views(
                View<ReCaptchaSettingsViewModel>("Admin.Edit.Header", model => recaptchaSettingsViewModel).Zone("header").Order(1),
                View<ReCaptchaSettingsViewModel>("Admin.Edit.Tools", model => recaptchaSettingsViewModel).Zone("tools").Order(1),
                View<ReCaptchaSettingsViewModel>("Admin.Edit.Content", model => recaptchaSettingsViewModel).Zone("content").Order(1)
            ));

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(ReCaptchaSettings viewModel, IViewProviderContext context)
        {
            throw new NotImplementedException();
        }
    }
}
