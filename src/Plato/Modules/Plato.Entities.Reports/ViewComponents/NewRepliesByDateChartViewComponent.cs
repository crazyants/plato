using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Models.Extensions;
using Plato.Reports.ViewModels;
using Plato.Entities.Repositories;
using Plato.Internal.Models.Metrics;

namespace Plato.Entities.Reports.ViewComponents
{
    public class NewRepliesByDateChartViewComponent : ViewComponent
    {
        private readonly IAggregatedEntityReplyRepository _aggregatedEntityReplyRepository;

        public NewRepliesByDateChartViewComponent(IAggregatedEntityReplyRepository aggregatedEntityReplyRepository)
        {
            _aggregatedEntityReplyRepository = aggregatedEntityReplyRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            ReportOptions options,
            ChartOptions chart)
        {
            
            if (options == null)
            {
                options = new ReportOptions();
            }

            if (chart == null)
            {
                chart = new ChartOptions();
            }


            var data = options.FeatureId > 0
                ? await _aggregatedEntityReplyRepository.SelectGroupedByDateAsync(
                    "CreatedDate",
                    options.Start,
                    options.End,
                    options.FeatureId)
                : await _aggregatedEntityReplyRepository.SelectGroupedByDateAsync(
                    "CreatedDate",
                    options.Start,
                    options.End);
            
            return View(new ChartViewModel<AggregatedResult<DateTimeOffset>>()
            {
                Options = chart,
                Data = data.MergeIntoRange(options.Start, options.End)
            });

        }

    }

}
