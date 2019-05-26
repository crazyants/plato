using System;
using System.Threading.Tasks;
using Plato.Features.Updates.ViewModels;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Features.Updates.ViewProviders
{
    
    public class AdminViewProvider : BaseViewProvider<FeatureUpdatesIndexViewModel>
    {

        public override Task<IViewProviderResult> BuildDisplayAsync(FeatureUpdatesIndexViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(FeatureUpdatesIndexViewModel indexViewModel, IViewProviderContext updater)
        {
            return Task.FromResult(Views(
                View<FeatureUpdatesIndexViewModel>("Admin.Index.Header", model => indexViewModel).Zone("header"),
                View<FeatureUpdatesIndexViewModel>("Admin.Index.Tools", model => indexViewModel).Zone("tools"),
                View<FeatureUpdatesIndexViewModel>("Admin.Index.Content", model => indexViewModel).Zone("content")
            ));
        }

        public override Task<IViewProviderResult> BuildEditAsync(FeatureUpdatesIndexViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(FeatureUpdatesIndexViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }

}
