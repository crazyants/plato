using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Models.Extensions;
using Plato.Internal.Repositories.Reputations;
using Plato.Reports.ViewModels;
using Plato.Metrics.Repositories;
using Plato.Internal.Repositories.Metrics;

namespace Plato.Reports.ViewComponents
{
    public class UserStatsByDateChartViewComponent : ViewComponent
    {

        private readonly IAggregatedUserRepository _aggregatedUserRepository;

        public UserStatsByDateChartViewComponent(IAggregatedUserRepository aggregatedUserRepository)
        {
            _aggregatedUserRepository = aggregatedUserRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReportOptions options)
        {
            
            if (options == null)
            {
                options = new ReportOptions();
            }
            
            var data = await _aggregatedUserRepository.SelectUserMetricsAsync(options.Start, options.End);
            return View(data);

        }

    }

}
