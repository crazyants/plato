using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Models.Extensions;
using Plato.Internal.Repositories.Metrics;
using Plato.Reports.ViewModels;

namespace Plato.Reports.ViewComponents
{
    public class NewUsersByDateChartViewComponent : ViewComponent
    {

        private readonly IAggregatedUserRepository _aggregatedUserRepository;

        public NewUsersByDateChartViewComponent(IAggregatedUserRepository aggregatedUserRepository)
        {
            _aggregatedUserRepository = aggregatedUserRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReportOptions options)
        {

            if (options == null)
            {
                options = new ReportOptions();
            }

            var users = await _aggregatedUserRepository.SelectGroupedByDateAsync("CreatedDate", options.Start, options.End);
            return View(users.MergeIntoRange(options.Start, options.End));

        }

    }

}
