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

        public override async Task<IViewProviderResult> BuildDisplayAsync(FeaturesViewModel model, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(FeaturesViewModel viewModel, IUpdateModel updater)
        {

            return Views(
                View<FeaturesViewModel>("Features.Index.Content", model => viewModel).Zone("content")
            );


        }

        public override async Task<IViewProviderResult> BuildEditAsync(FeaturesViewModel model, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(FeaturesViewModel model, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }
    }

}
