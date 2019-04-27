using System;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Metrics.Models;
using Plato.Reports.Models;
using Plato.Reports.ViewModels;
using Plato.Reports.PageViews.Models;

namespace Plato.Reports.PageViews.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<PageViewIndex>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(PageViewIndex viewReportModel, IViewProviderContext context)
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
                View<ReportIndexOptions>("Reports.Admin.Index.Tools", model => indexViewModel.Options).Zone("tools").Order(1),
                View<ReportIndexViewModel<Metric>>("Admin.Index.Content", model => indexViewModel).Zone("content").Order(1)
            ));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(PageViewIndex viewReportModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(PageViewIndex viewReportModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(PageViewIndex viewModel, IViewProviderContext context)
        {

            var model = new ReportIndexOptions();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildIndexAsync(viewModel, context);
            }

            if (context.Updater.ModelState.IsValid)
            {


            }

            return await BuildIndexAsync(viewModel, context);
            
        }

    }

}
