using Plato.Reports.ViewModels;

namespace Plato.Reports.PageViews.ViewModels
{
    public class MetricListItemViewModel<TModel> where TModel : class
    {

        public TModel Metric { get; set; }

        public ReportOptions Options { get; set; }

    }
}
