using System;
using Plato.Internal.Models.Metrics;

namespace Plato.Entities.Reporting.ViewModels
{
    public class EntitiesOverviewReportViewModel
    {

        public AggregatedResult<DateTimeOffset> Views { get; set; }

        public AggregatedResult<DateTimeOffset> Entities { get; set; }

        public AggregatedResult<DateTimeOffset> Replies { get; set; }
        
    }
}
