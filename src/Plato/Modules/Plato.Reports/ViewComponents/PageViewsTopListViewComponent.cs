using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Reports.ViewModels;
using Plato.Internal.Models.Metrics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Antiforgery.Internal;
using Plato.Internal.Data.Abstractions;
using Plato.Metrics.Repositories;

namespace Plato.Entities.Reports.ViewComponents
{
    public class PageViewsTopListViewComponent : ViewComponent
    {

        private readonly IAggregatedMetricsRepository _aggregatedMetricsRepository;
   
        public PageViewsTopListViewComponent(
            IAggregatedMetricsRepository aggregatedMetricsRepository)
        {
            _aggregatedMetricsRepository = aggregatedMetricsRepository;
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

            var data = await _aggregatedMetricsRepository.SelectGroupedByTitleAsync(
                options.Start,
                options.End,
                20);

            return View(data);

        }
        
     
    }

}
