using System;
using Plato.Internal.Models.Metrics;

namespace Plato.Discuss.Reporting.ViewModels
{
    public class DiscussOverviewReportViewModel
    {

        public AggregatedResult<DateTimeOffset> Entities { get; set; }

        public AggregatedResult<DateTimeOffset> Replies { get; set; }

    }
}
