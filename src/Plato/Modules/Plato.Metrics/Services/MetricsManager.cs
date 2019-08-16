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

        private readonly IMetricsStore<Metric> _metricsStore;
        private readonly IBroker _broker;

        public MetricsManager(
            IMetricsStore<Metric> metricsStore,
            IBroker broker)
        {
            _metricsStore = metricsStore;
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
            var metric = await _metricsStore.CreateAsync(model);
            if (metric != null)
            {

                // Invoke MetricCreated subscriptions
                foreach (var handler in _broker.Pub<Metric>(this, "MetricCreated", metric))
                {
                    metric = await handler.Invoke(new Message<Metric>(metric, this));
                }

                return result.Success(metric);

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
            var metric = await _metricsStore.UpdateAsync(model);
            if (metric != null)
            {

                // Invoke MetricUpdated subscriptions
                foreach (var handler in _broker.Pub<Metric>(this, "MetricUpdated", metric))
                {
                    metric = await handler.Invoke(new Message<Metric>(metric, this));
                }

                return result.Success(metric);

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

            var success = await _metricsStore.DeleteAsync(model);
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
