using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Admin.Models;
using Plato.Internal.Models.Extensions;
using Plato.Internal.Repositories.Metrics;
using Plato.Reporting.ViewModels;
using Plato.Internal.Repositories.Reputations;
using Plato.Metrics.Repositories;
using Plato.Reporting.Services;

namespace Plato.Reporting.ViewProviders
{

    public class AdminIndexViewProvider : BaseViewProvider<AdminIndex>
    {

        private readonly IAggregatedMetricsRepository _aggregatedMetricsRepository;
        private readonly IAggregatedUserRepository _aggregatedUserRepository;
        private readonly IAggregatedUserReputationRepository _aggregatedUserReputationRepository;
        private readonly IDateRangeStorage _dateRangeStorage;

        public AdminIndexViewProvider(
            IAggregatedMetricsRepository aggregatedMetricsRepository,
            IAggregatedUserRepository aggregatedUserRepository,
            IAggregatedUserReputationRepository aggregatedUserReputationRepository,
            IDateRangeStorage dateRangeStorage)
        {
            _aggregatedMetricsRepository = aggregatedMetricsRepository;
            _aggregatedUserRepository = aggregatedUserRepository;
            _aggregatedUserReputationRepository = aggregatedUserReputationRepository;
            _dateRangeStorage = dateRangeStorage;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel,
            IViewProviderContext context)
        {

            // Get range to display
            var range = _dateRangeStorage.Contextualize(context.Controller.ControllerContext);
       
            // Build index view model
            var reportIndexViewModel = new ReportIndexViewModel()
            {
                StartDate = range.Start,
                EndDate = range.End
            };
            
            // Get report data
            var pageViews = await _aggregatedMetricsRepository.SelectGroupedByDate("CreatedDate", range.Start, range.End);
            var pageViewsByFeature = await _aggregatedMetricsRepository.SelectGroupedByFeature(range.Start, range.End);
            var newUsers = await _aggregatedUserRepository.SelectGroupedByDate("CreatedDate", range.Start, range.End);
            var engagements = await _aggregatedUserReputationRepository.SelectGroupedByDate("CreatedDate", range.Start, range.End);

            // Build report view model
            var overviewViewModel = new OverviewReportViewModel()
            {
                PageViews = pageViews.MergeIntoRange(range.Start, range.End),
                PageViewsByFeature = pageViewsByFeature,
                NewUsers = newUsers.MergeIntoRange(range.Start, range.End),
                Engagements = engagements.MergeIntoRange(range.Start, range.End)
            };

            return Views(
                View<ReportIndexViewModel>("Reports.Admin.Index.Tools", model => reportIndexViewModel).Zone("tools")
                    .Order(int.MinValue),
                View<OverviewReportViewModel>("Reports.Overview", model => overviewViewModel).Zone("content")
                    .Order(int.MinValue)
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
