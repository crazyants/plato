using System;
using System.Collections.Generic;
using System.Text;
using Plato.Entities.Metrics.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Entities.Models;
using Plato.Internal.Models.Metrics;

namespace Plato.Entities.Reporting.ViewModels
{

    public class AggregatedEntityMetric<T>
    {

        public AggregatedCount<T> Aggregate { get; set; } = new AggregatedCount<T>();

        public Entity Entity { get; set; }


    }

    public class CombinedEntityMetricsViewModel<T>
    {

        public IEnumerable<AggregatedEntityMetric<T>> Results { get; set; }
        
    }
}
