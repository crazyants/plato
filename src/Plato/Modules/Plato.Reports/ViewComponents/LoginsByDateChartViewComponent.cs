using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Models.Extensions;
using Plato.Internal.Repositories.Metrics;
using Plato.Reports.ViewModels;

namespace Plato.Reports.ViewComponents
{
    public class LoginsByDateChartViewComponent : ViewComponent
    {

        private readonly IAggregatedUserRepository _aggregatedUserRepository;

        public LoginsByDateChartViewComponent(IAggregatedUserRepository aggregatedUserRepository)
        {
            _aggregatedUserRepository = aggregatedUserRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReportOptions options)
        {

            if (options == null)
            {
                options = new ReportOptions();
            }

            var data = await _aggregatedUserRepository.SelectGroupedByDateAsync("LastLoginDate", options.Start, options.End);
            return View(data.MergeIntoRange(options.Start, options.End));

        }

    }

}
