using System;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Admin.Models;
using Plato.Entities.Metrics.Repositories;
using Plato.Entities.Reporting.ViewModels;
using Plato.Entities.Repositories;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Extensions;
using Plato.Internal.Models.Metrics;

namespace Plato.Entities.Reporting.ViewProviders
{

    public class AdminIndexViewProvider : BaseViewProvider<AdminIndex>
    {
        
        private readonly IAggregatedEntityRepository _aggregatedEntityRepository;
        private readonly IAggregatedEntityMetricsRepository _aggregatedEntityMetricsRepository;
        private readonly IAggregatedEntityReplyRepository _aggregatedEntityReplyRepository;

        public AdminIndexViewProvider(
            IAggregatedEntityRepository aggregatedEntityRepository,
            IAggregatedEntityReplyRepository aggregatedEntityReplyRepository,
            IAggregatedEntityMetricsRepository aggregatedEntityMetricsRepository)
        {
            _aggregatedEntityRepository = aggregatedEntityRepository;
            _aggregatedEntityReplyRepository = aggregatedEntityReplyRepository;
            _aggregatedEntityMetricsRepository = aggregatedEntityMetricsRepository;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel,
            IViewProviderContext context)
        {

            var start = DateTimeOffset.UtcNow.AddDays(-7);
            var end = DateTimeOffset.UtcNow;
            
            var entities = await _aggregatedEntityRepository.SelectGroupedByDate("CreatedDate", start, end);
            var replies = await _aggregatedEntityReplyRepository.SelectGroupedByDate("CreatedDate", start, end);
            var views = await _aggregatedEntityMetricsRepository.SelectGroupedByDate("CreatedDate", start, end);

            var overviewViewModel = new EntitiesOverviewReportViewModel()
            {
                Entities = entities.MergeIntoRange(start, end),
                Replies = replies.MergeIntoRange(start, end),
                Views = views.MergeIntoRange(start, end)
            };

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
        
        private AggregatedResult<DateTimeOffset> BuildChartData(
            DateTimeOffset start,
            DateTimeOffset end)
        {

            var delta = start.DayDifference(end);
            var output = new AggregatedResult<DateTimeOffset>();
            for (var i = delta; i > 0; i--)
            {
                output.Data.Add(new AggregatedCount<DateTimeOffset>()
                {
                    Aggregate = end.AddDays(i),
                    Count = 0
                });
            }

            return output;

        }
        
    }
    
}
