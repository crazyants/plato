using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Models.Extensions;
using Plato.Reports.ViewModels;
using Plato.Entities.Repositories;
using Plato.Internal.Models.Metrics;

namespace Plato.Entities.Reports.ViewComponents
{
    public class NewByDateChartViewComponent : ViewComponent
    {

        private readonly IAggregatedEntityRepository _aggregatedEntityRepository;
        private readonly IAggregatedEntityReplyRepository _aggregatedEntityReplyRepository;


        public NewByDateChartViewComponent(
            IAggregatedEntityRepository aggregatedEntityRepository,
            IAggregatedEntityReplyRepository aggregatedEntityReplyRepository)
        {
            _aggregatedEntityRepository = aggregatedEntityRepository;
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
            
            var entities = options.FeatureId > 0
                ? await _aggregatedEntityRepository.SelectGroupedByDateAsync(
                    "CreatedDate",
                    options.Start,
                    options.End,
                    options.FeatureId)
                : await _aggregatedEntityRepository.SelectGroupedByDateAsync(
                    "CreatedDate",
                    options.Start,
                    options.End);
            
            var replies = options.FeatureId > 0
                ? await _aggregatedEntityReplyRepository.SelectGroupedByDateAsync(
                    "CreatedDate",
                    options.Start,
                    options.End,
                    options.FeatureId)
                : await _aggregatedEntityReplyRepository.SelectGroupedByDateAsync(
                    "CreatedDate",
                    options.Start,
                    options.End);


            var results = new Dictionary<string, ChartViewModel<AggregatedResult<DateTimeOffset>>>();
            results.Add("entities", new ChartViewModel<AggregatedResult<DateTimeOffset>>()
            {
                Options = chart,
                Data = entities.MergeIntoRange(options.Start, options.End)
            });
            results.Add("replies", new ChartViewModel<AggregatedResult<DateTimeOffset>>()
            {
                Options = chart,
                Data = replies.MergeIntoRange(options.Start, options.End)
            });
            
            return View(results);

        }

    }

}
