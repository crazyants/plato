using System;
using System.Threading.Tasks;
using Plato.Features.ViewModels;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Features.ViewProviders
{
    
    public class AdminViewProvider : BaseViewProvider<FeaturesViewModel>
    {

        public override Task<IViewProviderResult> BuildDisplayAsync(FeaturesViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(FeaturesViewModel viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(Views(
                View<FeaturesViewModel>("Admin.Index.Header", model => viewModel).Zone("header"),
                View<FeaturesViewModel>("Admin.Index.Content", model => viewModel).Zone("content")
            ));
        }

        public override Task<IViewProviderResult> BuildEditAsync(FeaturesViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(FeaturesViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }

}
