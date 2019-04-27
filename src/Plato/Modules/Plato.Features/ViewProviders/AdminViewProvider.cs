using System;
using System.Threading.Tasks;
using Plato.Features.ViewModels;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Features.ViewProviders
{
    
    public class AdminViewProvider : BaseViewProvider<FeaturesIndexViewModel>
    {

        public override Task<IViewProviderResult> BuildDisplayAsync(FeaturesIndexViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(FeaturesIndexViewModel indexViewModel, IViewProviderContext updater)
        {
            return Task.FromResult(Views(
                View<FeaturesIndexViewModel>("Admin.Index.Header", model => indexViewModel).Zone("header"),
                View<FeaturesIndexViewModel>("Admin.Index.Tools", model => indexViewModel).Zone("tools"),
                View<FeaturesIndexViewModel>("Admin.Index.Content", model => indexViewModel).Zone("content")
            ));
        }

        public override Task<IViewProviderResult> BuildEditAsync(FeaturesIndexViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(FeaturesIndexViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }

}
