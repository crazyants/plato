using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Models.Extensions;
using Plato.Reports.ViewModels;
using Plato.Entities.Repositories;

namespace Plato.Entities.Reports.ViewComponents
{
    public class NewRepliesByDateChartViewComponent : ViewComponent
    {
        private readonly IAggregatedEntityReplyRepository _aggregatedEntityReplyRepository;

        public NewRepliesByDateChartViewComponent(IAggregatedEntityReplyRepository aggregatedEntityReplyRepository)
        {
            _aggregatedEntityReplyRepository = aggregatedEntityReplyRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReportIndexOptions options)
        {
            
            if (options == null)
            {
                options = new ReportIndexOptions();
            }

            var data = await _aggregatedEntityReplyRepository.SelectGroupedByDateAsync("CreatedDate", options.Start,
                options.End);
            return View(data.MergeIntoRange(options.Start, options.End));

        }

    }

}
