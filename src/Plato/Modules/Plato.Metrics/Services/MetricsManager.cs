using System;
using System.Threading.Tasks;
using Plato.Metrics.Models;
using Plato.Metrics.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Metrics.Services
{

    public class MetricsManager : IMetricsManager<Metric>
    {

        private readonly IMetricsStore<Metric> _entityRatingsStore;

        private readonly IBroker _broker;

        public MetricsManager(
            IMetricsStore<Metric> entityRatingsStore,
            IBroker broker)
        {
            _entityRatingsStore = entityRatingsStore;
            _broker = broker;
        }

        public async Task<ICommandResult<Metric>> CreateAsync(Metric model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }
            
            // Invoke MetricCreating subscriptions
            foreach (var handler in _broker.Pub<Metric>(this, "MetricCreating", model))
            {
                model = await handler.Invoke(new Message<Metric>(model, this));
            }

            // Create result
            var result = new CommandResult<Metric>();

            // Attempt to persist
            var reaction = await _entityRatingsStore.CreateAsync(model);
            if (reaction != null)
            {

                // Invoke MetricCreated subscriptions
                foreach (var handler in _broker.Pub<Metric>(this, "MetricCreated", reaction))
                {
                    reaction = await handler.Invoke(new Message<Metric>(reaction, this));
                }

                return result.Success(reaction);

            }

            return result.Failed($"An unknown error occurred whilst attempting to create a metric");

        }

        public async Task<ICommandResult<Metric>> UpdateAsync(Metric model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            // Invoke MetricUpdating subscriptions
            foreach (var handler in _broker.Pub<Metric>(this, "MetricUpdating", model))
            {
                model = await handler.Invoke(new Message<Metric>(model, this));
            }

            // Create result
            var result = new CommandResult<Metric>();

            // Attempt to persist
            var reaction = await _entityRatingsStore.UpdateAsync(model);
            if (reaction != null)
            {

                // Invoke MetricUpdated subscriptions
                foreach (var handler in _broker.Pub<Metric>(this, "MetricUpdated", reaction))
                {
                    reaction = await handler.Invoke(new Message<Metric>(reaction, this));
                }

                return result.Success(reaction);

            }

            return result.Failed($"An unknown error occurred whilst attempting to update a metric");

        }

        public async Task<ICommandResult<Metric>> DeleteAsync(Metric model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Invoke MetricDeleting subscriptions
            foreach (var handler in _broker.Pub<Metric>(this, "MetricDeleting", model))
            {
                model = await handler.Invoke(new Message<Metric>(model, this));
            }

            var result = new CommandResult<Metric>();

            var success = await _entityRatingsStore.DeleteAsync(model);
            if (success)
            {

                // Invoke MetricDeleted subscriptions
                foreach (var handler in _broker.Pub<Metric>(this, "MetricDeleted", model))
                {
                    model = await handler.Invoke(new Message<Metric>(model, this));
                }

                return result.Success(model);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the metric."));

        }

    }

}
