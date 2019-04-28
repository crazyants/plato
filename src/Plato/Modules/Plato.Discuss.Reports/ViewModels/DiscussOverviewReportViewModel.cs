using System;
using Plato.Internal.Models.Metrics;

namespace Plato.Discuss.Reports.ViewModels
{
    public class DiscussOverviewReportViewModel
    {

        public AggregatedResult<DateTimeOffset> Entities { get; set; }

        public AggregatedResult<DateTimeOffset> Replies { get; set; }

        public AggregatedResult<DateTimeOffset> Views { get; set; }

    }

}
