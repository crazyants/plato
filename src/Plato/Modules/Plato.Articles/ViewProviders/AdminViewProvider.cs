using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Articles.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<AdminHome>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(AdminHome viewModel, IViewProviderContext context)
        {
            return Task.FromResult(Views(
                View<AdminHome>("Admin.Index.Header", model => viewModel).Zone("header").Order(1),
                View<AdminHome>("Admin.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<AdminHome>("Admin.Index.Content", model => viewModel).Zone("content").Order(1)
            ));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(AdminHome viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(AdminHome viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(AdminHome viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
