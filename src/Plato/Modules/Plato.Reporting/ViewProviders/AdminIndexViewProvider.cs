using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Admin.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Extensions;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Repositories.Metrics;
using Plato.Reporting.ViewModels;
using Plato.Internal.Repositories.Reputations;

namespace Plato.Reporting.ViewProviders
{

    public class AdminIndexViewProvider : BaseViewProvider<AdminIndex>
    {

        private readonly IAggregatedMetricsRepository _aggregatedMetricsRepository;
        private readonly IAggregatedUserMetricsRepository _aggregatedUserMetricsRepository;
        private readonly IAggregatedReputationMetricsRepository _aggregatedReputationMetricsRepository;

        public AdminIndexViewProvider(
            IAggregatedMetricsRepository aggregatedMetricsRepository,
            IAggregatedUserMetricsRepository aggregatedUserMetricsRepository,
            IAggregatedReputationMetricsRepository aggregatedReputationMetricsRepository)
        {
            _aggregatedMetricsRepository = aggregatedMetricsRepository;
            _aggregatedUserMetricsRepository = aggregatedUserMetricsRepository;
            _aggregatedReputationMetricsRepository = aggregatedReputationMetricsRepository;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel,
            IViewProviderContext context)
        {

            var start = DateTimeOffset.UtcNow.AddDays(-7);
            var end = DateTimeOffset.UtcNow;
            

            var pageViews = await _aggregatedMetricsRepository.SelectGroupedByDate("CreatedDate", start, end);
            var newUsers = await _aggregatedUserMetricsRepository.SelectGroupedByDate("CreatedDate", start, end);
            var engagements = await _aggregatedReputationMetricsRepository.SelectGroupedByDate("CreatedDate", start, end);
            
            var overviewViewModel = new OverviewReportViewModel()
            {
                PageViews = pageViews.MergeIntoRange(start, end),
                NewUsers = newUsers.MergeIntoRange(start, end),
                Engagements = engagements.MergeIntoRange(start, end)
            };

            return Views(
                View<OverviewReportViewModel>("Overview.Report", model => overviewViewModel).Zone("content")
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
