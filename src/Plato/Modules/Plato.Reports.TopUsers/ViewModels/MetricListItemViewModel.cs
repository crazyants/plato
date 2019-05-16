using Plato.Reports.ViewModels;

namespace Plato.Reports.TopUsers.ViewModels
{
    public class MetricListItemViewModel<TModel> where TModel : class
    {

        public TModel Metric { get; set; }

        public ReportOptions Options { get; set; }

    }
}
