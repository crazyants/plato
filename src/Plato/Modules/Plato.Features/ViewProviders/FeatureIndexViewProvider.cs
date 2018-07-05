using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Features.ViewModels;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Features.ViewProviders
{
    
    public class FeaturesIndexViewProvider : BaseViewProvider<FeaturesViewModel>
    {

        public override Task<IViewProviderResult> BuildDisplayAsync(FeaturesViewModel model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(FeaturesViewModel viewModel, IUpdateModel updater)
        {

            return Task.FromResult(Views(
                View<FeaturesViewModel>("Features.Index.Header", model => viewModel).Zone("header"),
                View<FeaturesViewModel>("Features.Index.Content", model => viewModel).Zone("content")
            ));


        }

        public override Task<IViewProviderResult> BuildEditAsync(FeaturesViewModel model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(FeaturesViewModel model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }

}
