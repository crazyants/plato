using System;
using System.Threading.Tasks;
using Plato.Core.Models;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Core.ViewProviders
{
    public class HomeViewProvider : BaseViewProvider<HomeIndex>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            
            return Task.FromResult(Views(
                View<HomeIndex>("Home.Index.Header", model => viewModel).Zone("header"),
                View<HomeIndex>("Home.Index.Tools", model => viewModel).Zone("tools"),
                View<HomeIndex>("Home.Index.Content", model => viewModel).Zone("content")
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
