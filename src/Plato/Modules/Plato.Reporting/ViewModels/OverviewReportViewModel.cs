using System;
using Plato.Internal.Models.Metrics;

namespace Plato.Reporting.ViewModels
{
    public class OverviewReportViewModel
    {

        public AggregatedResult<DateTimeOffset> PageViews { get; set; }

        public AggregatedResult<DateTimeOffset> NewUsers { get; set; }

        public AggregatedResult<DateTimeOffset> Engagements { get; set; }
    }
}
