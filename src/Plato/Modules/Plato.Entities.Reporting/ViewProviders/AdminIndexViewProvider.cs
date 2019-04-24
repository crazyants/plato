using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Admin.Models;
using Plato.Entities.Metrics.Repositories;
using Plato.Entities.Reporting.ViewModels;
using Plato.Entities.Repositories;
using Plato.Internal.Models.Extensions;
using Plato.Reporting.Services;

namespace Plato.Entities.Reporting.ViewProviders
{

    public class AdminIndexViewProvider : BaseViewProvider<AdminIndex>
    {
        
        private readonly IAggregatedEntityRepository _aggregatedEntityRepository;
        private readonly IAggregatedEntityMetricsRepository _aggregatedEntityMetricsRepository;
        private readonly IAggregatedEntityReplyRepository _aggregatedEntityReplyRepository;
        private readonly IDateRangeStorage _dateRangeStorage;

        public AdminIndexViewProvider(
            IAggregatedEntityRepository aggregatedEntityRepository,
            IAggregatedEntityReplyRepository aggregatedEntityReplyRepository,
            IAggregatedEntityMetricsRepository aggregatedEntityMetricsRepository,
            IDateRangeStorage dateRangeStorage)
        {
            _aggregatedEntityRepository = aggregatedEntityRepository;
            _aggregatedEntityReplyRepository = aggregatedEntityReplyRepository;
            _aggregatedEntityMetricsRepository = aggregatedEntityMetricsRepository;
            _dateRangeStorage = dateRangeStorage;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel,
            IViewProviderContext context)
        {

            // Get range to display
            var range = _dateRangeStorage.Contextualize(context.Controller.ControllerContext);

            // Get data
            var entities = await _aggregatedEntityRepository.SelectGroupedByDate("CreatedDate", range.Start, range.End);
            var replies = await _aggregatedEntityReplyRepository.SelectGroupedByDate("CreatedDate", range.Start, range.End);
            var views = await _aggregatedEntityMetricsRepository.SelectGroupedByDate("CreatedDate", range.Start, range.End);

            // Build model
            var overviewViewModel = new EntitiesOverviewReportViewModel()
            {
                Entities = entities.MergeIntoRange(range.Start, range.End),
                Replies = replies.MergeIntoRange(range.Start, range.End),
                Views = views.MergeIntoRange(range.Start, range.End)
            };

            // Return view
            return Views(
                View<EntitiesOverviewReportViewModel>("Entities.Overview.Report", model => overviewViewModel).Zone("content").Order(1)
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
            return await BuildEditAsync(viewModel, context);
        }
        
    }
    
}
