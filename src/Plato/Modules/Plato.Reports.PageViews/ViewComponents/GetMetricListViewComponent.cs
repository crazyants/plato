using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Navigation.Abstractions;
using Plato.Metrics.Stores;
using Plato.Metrics.Models;
using Plato.Reports.ViewModels;

namespace Plato.Reports.PageViews.ViewComponents
{
    public class GetMetricListViewComponent : ViewComponent
    {

        private readonly IMetricsStore<Metric> _metricStore;

        public GetMetricListViewComponent(IMetricsStore<Metric> metricStore)
        {
            _metricStore = metricStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReportOptions options, PagerOptions pager)
        {

            // Build default
            if (options == null)
            {
                options = new ReportOptions();
            }

            // Build default
            if (pager == null)
            {
                pager = new PagerOptions();
            }
        
            // Review view
            return View(await GetViewModel(options, pager));

        }

        async Task<ReportIndexViewModel<Metric>> GetViewModel(
            ReportOptions options,
            PagerOptions pager)
        {

            // Get results
            var results = await _metricStore.QueryAsync()
                .Take(pager.Page, pager.Size)
                .Select<MetricQueryParams>(q =>
                {
                    q.StartDate.GreaterThanOrEqual(options.Start);
                    q.EndDate.LessThanOrEqual(options.End);
                })
                .OrderBy(options.Sort.ToString(), options.Order)
                .ToList();

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new ReportIndexViewModel<Metric>
            {
                Results = results,
                Options = options,
                Pager = pager
            };

        }

    }
    
}

