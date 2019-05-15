using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Models.Extensions;
using Plato.Internal.Repositories.Reputations;
using Plato.Reports.ViewModels;
using Plato.Metrics.Repositories;

namespace Plato.Reports.ViewComponents
{
    public class UserStatsByDateChartViewComponent : ViewComponent
    {

        private readonly IAggregatedMetricsRepository _aggregatedUserReputationRepository;

        public UserStatsByDateChartViewComponent(IAggregatedMetricsRepository aggregatedUserReputationRepository)
        {
            _aggregatedUserReputationRepository = aggregatedUserReputationRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReportOptions options)
        {
            
            if (options == null)
            {
                options = new ReportOptions();
            }


            var data = await _aggregatedUserReputationRepository.SelectMetricsAsync(options.Start, options.End);
            return View(data);

        }

    }

}
