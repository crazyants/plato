using System;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Admin.Models;
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

            var start = DateTimeOffset.UtcNow.AddDays(-60);
            var end = DateTimeOffset.UtcNow;
            
            var overviewViewModel = new OverviewReportViewModel()
            {
                PageViews = await _aggregatedMetricsRepository.SelectGroupedByDate("CreatedDate", start, end),
                NewUsers = await _aggregatedUserMetricsRepository.SelectGroupedByDate("CreatedDate", start, end),
                Engagements = await _aggregatedReputationMetricsRepository.SelectGroupedByDate("CreatedDate", start, end)
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
        
    }
    
}
