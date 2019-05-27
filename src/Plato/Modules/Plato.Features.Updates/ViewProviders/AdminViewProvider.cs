using System;
using System.Threading.Tasks;
using Plato.Features.Updates.ViewModels;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Features.Updates.ViewProviders
{
    
    public class AdminViewProvider : BaseViewProvider<FeatureUpdatesViewModel>
    {

        public override Task<IViewProviderResult> BuildDisplayAsync(FeatureUpdatesViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(FeatureUpdatesViewModel viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(Views(
                View<FeatureUpdatesViewModel>("Admin.Index.Header", model => viewModel).Zone("header"),
                View<FeatureUpdatesViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools"),
                View<FeatureUpdatesViewModel>("Admin.Index.Content", model => viewModel).Zone("content")
            ));
        }

        public override Task<IViewProviderResult> BuildEditAsync(FeatureUpdatesViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(FeatureUpdatesViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }

}
