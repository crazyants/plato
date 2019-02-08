using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class RegisterViewProvider : BaseViewProvider<RegisterViewModel>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(RegisterViewModel viewModel, IViewProviderContext context)
        {

            return Task.FromResult(Views(
                View<RegisterViewModel>("Register.Index.Header", model => viewModel).Zone("header"),
                View<RegisterViewModel>("Register.Index.Content", model => viewModel).Zone("content"),
                View<RegisterViewModel>("Register.Index.Sidebar", model => viewModel).Zone("sidebar"),
                View<RegisterViewModel>("Register.Index.Footer", model => viewModel).Zone("footer")
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
