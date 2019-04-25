using System;

namespace Plato.Internal.Models.Metrics
{

    public class AggregatedModel<TAggregate, TModel> where TModel : class
    {
        public AggregatedCount<TAggregate> Aggregate { get; private set; }

        public TModel Model { get; private set; }

        public AggregatedModel(AggregatedCount<TAggregate> aggregate, TModel model)
        {
            Aggregate = aggregate ?? throw new ArgumentNullException(nameof(aggregate));
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }
    }
    
}
