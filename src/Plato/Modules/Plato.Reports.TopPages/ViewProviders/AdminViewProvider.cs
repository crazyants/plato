using System;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Metrics.Models;
using Plato.Reports.Models;
using Plato.Reports.ViewModels;
using Plato.Reports.TopPages.Models;
using Plato.Reports.Services;

namespace Plato.Reports.TopPages.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<FeatureViewIndex>
    {

        private readonly IDateRangeStorage _dateRangeStorage;

        public AdminViewProvider(IDateRangeStorage dateRangeStorage)
        {
            _dateRangeStorage = dateRangeStorage;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(FeatureViewIndex viewReportModel, IViewProviderContext context)
        {

            // Get range to display
            var range = _dateRangeStorage.Contextualize(context.Controller.ControllerContext);

            // Build index view model
            var reportIndexOptions = new ReportOptions()
            {
                Start = range.Start,
                End = range.End
            };

            return Task.FromResult(Views(
                View<ReportOptions>("Admin.Index.Header", model => reportIndexOptions).Zone("header").Order(1),
                View<ReportOptions>("Admin.Index.Tools", model => reportIndexOptions).Zone("tools").Order(1),
                View<ReportOptions>("Admin.Index.Content", model => reportIndexOptions).Zone("content").Order(1)
            ));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(FeatureViewIndex viewReportModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(FeatureViewIndex viewReportModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(FeatureViewIndex viewModel, IViewProviderContext context)
        {

            var model = new ReportOptions();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildIndexAsync(viewModel, context);
            }

            if (context.Updater.ModelState.IsValid)
            {
                var storage = _dateRangeStorage.Contextualize(context.Controller.ControllerContext);
                storage.Set(model.Start, model.End);
            }

            return await BuildIndexAsync(viewModel, context);

        }

    }

}
