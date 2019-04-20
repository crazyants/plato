using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Admin.Models;

namespace Plato.Reporting.ViewProviders
{

    public class AdminViewProvider : BaseViewProvider<AdminIndex>
    {
        
        public AdminViewProvider()
        {

        }

        public override Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel, IViewProviderContext context)
        {

            return Task.FromResult(Views(
                View<AdminIndex>("Reporting.PageViews.LineChart", model => viewModel).Zone("content").Order(int.MinValue)
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(AdminIndex viewModel,
            IViewProviderContext context)
        {
            return await BuildEditAsync(viewModel, context);
        }
        
    }


}
