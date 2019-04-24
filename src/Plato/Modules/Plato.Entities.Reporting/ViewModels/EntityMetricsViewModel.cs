using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Internal.Models.Metrics;

namespace Plato.Entities.Reporting.ViewModels
{

    public class AggregatedEntityMetric<T>
    {
        public AggregatedCount<T> Aggregate { get; private set; }

        public Entity Entity { get; private set; }

        public AggregatedEntityMetric(AggregatedCount<T> aggregate, Entity entity)
        {
            Aggregate = aggregate;
            Entity = Entity;
        }
    }

    public class EntityMetricsViewModel<T>
    {

        public IEnumerable<AggregatedEntityMetric<T>> Results { get; set; }
        
    }
}
