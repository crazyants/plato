using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Metrics.Models;
using Plato.Reports.ViewModels;

namespace Plato.Reports.ViewComponents
{
    public class MetricListItemViewComponent : ViewComponent
    {
  
        public MetricListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(MetricListItemViewModel<Metric> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

