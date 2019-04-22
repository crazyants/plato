using System;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Internal.Layout.ViewProviders;
using Plato.Discuss.Reporting.ViewModels;
using Plato.Entities.Repositories;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Extensions;
using Plato.Internal.Models.Metrics;

namespace Plato.Discuss.Reporting.ViewProviders
{

    public class AdminIndexViewProvider : BaseViewProvider<AdminIndex>
    {


        private readonly IAggregatedEntityRepository _aggregatedEntityRepository;
        private readonly IAggregatedEntityReplyRepository _aggregatedEntityReplyRepository;

        public AdminIndexViewProvider(
            IAggregatedEntityRepository aggregatedEntityRepository,
            IAggregatedEntityReplyRepository aggregatedEntityReplyRepository)
        {
            _aggregatedEntityRepository = aggregatedEntityRepository;
            _aggregatedEntityReplyRepository = aggregatedEntityReplyRepository;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel,
            IViewProviderContext context)
        {

            var start = DateTimeOffset.UtcNow.AddDays(-7);
            var end = DateTimeOffset.UtcNow;
            
            var entities = await _aggregatedEntityRepository.SelectGroupedByDate("CreatedDate", start, end);
            var replies = await _aggregatedEntityReplyRepository.SelectGroupedByDate("CreatedDate", start, end);
            
            var overviewViewModel = new DiscussOverviewReportViewModel()
            {
                Entities = entities.MergeIntoRange(start, end),
                Replies = replies.MergeIntoRange(start, end),
            };

            return Views(
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
