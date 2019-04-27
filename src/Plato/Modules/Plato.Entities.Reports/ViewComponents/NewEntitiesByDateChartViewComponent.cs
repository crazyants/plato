using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Models.Extensions;
using Plato.Reports.ViewModels;
using Plato.Entities.Repositories;

namespace Plato.Entities.Reports.ViewComponents
{
    public class NewEntitiesByDateChartViewComponent : ViewComponent
    {
        private readonly IAggregatedEntityRepository _aggregatedEntityRepository;

        public NewEntitiesByDateChartViewComponent(IAggregatedEntityRepository aggregatedEntityRepository)
        {
            _aggregatedEntityRepository = aggregatedEntityRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReportIndexOptions options)
        {
            
            if (options == null)
            {
                options = new ReportIndexOptions();
            }

            var data = await _aggregatedEntityRepository.SelectGroupedByDateAsync("CreatedDate", options.Start,
                options.End);
            return View(data.MergeIntoRange(options.Start, options.End));

        }

    }

}
