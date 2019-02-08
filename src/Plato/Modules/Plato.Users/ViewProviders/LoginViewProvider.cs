using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class LoginViewProvider : BaseViewProvider<LoginViewModel>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(LoginViewModel viewModel, IViewProviderContext context)
        {

            return Task.FromResult(Views(
                View<LoginViewModel>("Login.Index.Header", model => viewModel).Zone("header"),
                View<LoginViewModel>("Login.Index.Content", model => viewModel).Zone("content"),
                View<LoginViewModel>("Login.Index.Sidebar", model => viewModel).Zone("sidebar"),
                View<LoginViewModel>("Login.Index.Footer", model => viewModel).Zone("footer")
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(LoginViewModel viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));

        }
        
        public override Task<IViewProviderResult> BuildEditAsync(LoginViewModel viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(LoginViewModel viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));

        }
    }
}
