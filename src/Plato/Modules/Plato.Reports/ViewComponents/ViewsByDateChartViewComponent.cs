using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Models.Extensions;
using Plato.Metrics.Repositories;
using Plato.Reports.ViewModels;

namespace Plato.Reports.ViewComponents
{
    public class ViewsByDateChartViewComponent : ViewComponent
    {

        private readonly IAggregatedMetricsRepository _aggregatedMetricsRepository;

        public ViewsByDateChartViewComponent(IAggregatedMetricsRepository aggregatedMetricsRepository)
        {
            _aggregatedMetricsRepository = aggregatedMetricsRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReportOptions options)
        {
            
            if (options == null)
            {
                options = new ReportOptions();
            }

            var views = await _aggregatedMetricsRepository.SelectGroupedByDateAsync("CreatedDate", options.Start, options.End);
            return View(views.MergeIntoRange(options.Start, options.End));

        }

    }

}
