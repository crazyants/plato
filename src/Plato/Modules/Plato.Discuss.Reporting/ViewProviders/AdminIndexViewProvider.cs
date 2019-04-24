using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Internal.Layout.ViewProviders;
using Plato.Discuss.Reporting.ViewModels;
using Plato.Entities.Metrics.Repositories;
using Plato.Entities.Repositories;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Extensions;
using Plato.Reporting.Services;
using Plato.Reporting.ViewModels;

namespace Plato.Discuss.Reporting.ViewProviders
{

    public class AdminIndexViewProvider : BaseViewProvider<AdminIndex>
    {

        private readonly IAggregatedEntityMetricsRepository _aggregatedEntityMetricsRepository;
        private readonly IAggregatedEntityRepository _aggregatedEntityRepository;
        private readonly IAggregatedEntityReplyRepository _aggregatedEntityReplyRepository;
        private readonly IDateRangeStorage _dateRangeStorage;
        private readonly IFeatureFacade _featureFacade;

        public AdminIndexViewProvider(
            IAggregatedEntityRepository aggregatedEntityRepository,
            IAggregatedEntityReplyRepository aggregatedEntityReplyRepository,
            IAggregatedEntityMetricsRepository aggregatedEntityMetricsRepository,
            IDateRangeStorage dateRangeStorage,
            IFeatureFacade featureFacade)
        {
            _aggregatedEntityRepository = aggregatedEntityRepository;
            _aggregatedEntityReplyRepository = aggregatedEntityReplyRepository;
            _aggregatedEntityMetricsRepository = aggregatedEntityMetricsRepository;
            _dateRangeStorage = dateRangeStorage;
            _featureFacade = featureFacade;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel,
            IViewProviderContext context)
        {

            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");

            // Get range to display
            var range = _dateRangeStorage.Contextualize(context.Controller.ControllerContext);
            
            // Build index view model
            var reportIndexViewModel = new ReportIndexViewModel()
            {
                StartDate = range.Start,
                EndDate = range.End
            };


            // Get data
            var entities = await _aggregatedEntityRepository.SelectGroupedByDate(
                "CreatedDate",
                range.Start,
                range.End,
                feature?.Id ?? 0);

            var replies = await _aggregatedEntityReplyRepository.SelectGroupedByDate(
                "CreatedDate",
                range.Start,
                range.End,
                feature?.Id ?? 0);

            var views = await _aggregatedEntityMetricsRepository.SelectGroupedByDate(
                "CreatedDate",
                range.Start,
                range.End,
                feature?.Id ?? 0);
            
            var overviewViewModel = new DiscussOverviewReportViewModel()
            {
                Entities = entities.MergeIntoRange(range.Start, range.End),
                Replies = replies.MergeIntoRange(range.Start, range.End),
                Views = views.MergeIntoRange(range.Start, range.End)
            };

            return Views(
                View<ReportIndexViewModel>("Reports.Admin.Index.Tools", model => reportIndexViewModel).Zone("tools")
                    .Order(int.MinValue),
                View<DiscussOverviewReportViewModel>("Discuss.Overview.Report", model => overviewViewModel).Zone("content").Order(1)
                    .Order(1)
            );

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

            var model = new ReportIndexViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildIndexAsync(viewModel, context);
            }

            if (context.Updater.ModelState.IsValid)
            {
                var storage = _dateRangeStorage.Contextualize(context.Controller.ControllerContext);
                storage.Set(model.StartDate, model.EndDate);
            }

            return await BuildIndexAsync(viewModel, context);

        }
        
    }
    
}
