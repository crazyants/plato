using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Models.Extensions;
using Plato.Reports.ViewModels;
using Plato.Entities.Metrics.Repositories;

namespace Plato.Entities.Reports.ViewComponents
{
    public class EntityViewsByDateChartViewComponent : ViewComponent
    {

        private readonly IAggregatedEntityMetricsRepository _aggregatedEntityMetricsRepository;

        public EntityViewsByDateChartViewComponent(IAggregatedEntityMetricsRepository aggregatedEntityMetricsRepository)
        {
            _aggregatedEntityMetricsRepository = aggregatedEntityMetricsRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReportIndexOptions options)
        {
            
            if (options == null)
            {
                options = new ReportIndexOptions();
            }

            var views = await _aggregatedEntityMetricsRepository.SelectGroupedByDateAsync("CreatedDate", options.Start,
                options.End);
            return View(views.MergeIntoRange(options.Start, options.End));

        }

    }

}
