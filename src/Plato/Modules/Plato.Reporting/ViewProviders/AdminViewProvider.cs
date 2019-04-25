using System;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Metrics.Models;
using Plato.Reporting.Models;
using Plato.Reporting.ViewModels;

namespace Plato.Reporting.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<Report>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(Report viewModel, IViewProviderContext context)
        {

            // Get view model from context
            var indexViewModel =
                context.Controller.HttpContext.Items[typeof(ReportIndexViewModel<Metric>)] as ReportIndexViewModel<Metric>;
            
            // Ensure we have the view model
            if (indexViewModel == null)
            {
                throw new Exception("No type of ReportIndexViewModel has been registered with HttpContext.Items");
            }

            return Task.FromResult(Views(
                View<ReportIndexViewModel<Metric>>("Admin.Index.Header", model => indexViewModel).Zone("header").Order(1),
                View<ReportIndexViewModel<Metric>>("Admin.Index.Tools", model => indexViewModel).Zone("tools").Order(1),
                View<ReportIndexViewModel<Metric>>("Admin.Index.Content", model => indexViewModel).Zone("content").Order(1)
            ));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Report viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Report viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Report viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
