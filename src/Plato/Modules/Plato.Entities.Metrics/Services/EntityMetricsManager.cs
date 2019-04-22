using System;
using System.Threading.Tasks;
using Plato.Entities.Metrics.Models;
using Plato.Entities.Metrics.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Entities.Metrics.Services
{

    public class EntityMetricsManager : IEntityMetricsManager<EntityMetric>
    {

        private readonly IEntityMetricsStore<EntityMetric> _entityRatingsStore;

        private readonly IBroker _broker;

        public EntityMetricsManager(
            IEntityMetricsStore<EntityMetric> entityRatingsStore,
            IBroker broker)
        {
            _entityRatingsStore = entityRatingsStore;
            _broker = broker;
        }

        public async Task<ICommandResult<EntityMetric>> CreateAsync(EntityMetric model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            // Invoke EntityMetricCreating subscriptions
            foreach (var handler in _broker.Pub<EntityMetric>(this, "EntityMetricCreating", model))
            {
                model = await handler.Invoke(new Message<EntityMetric>(model, this));
            }

            // Create result
            var result = new CommandResult<EntityMetric>();

            // Attempt to persist
            var reaction = await _entityRatingsStore.CreateAsync(model);
            if (reaction != null)
            {

                // Invoke EntityMetricCreated subscriptions
                foreach (var handler in _broker.Pub<EntityMetric>(this, "EntityMetricCreated", reaction))
                {
                    reaction = await handler.Invoke(new Message<EntityMetric>(reaction, this));
                }

                return result.Success(reaction);

            }

            return result.Failed($"An unknown error occurred whilst attempting to create an entity metric");

        }

        public async Task<ICommandResult<EntityMetric>> UpdateAsync(EntityMetric model)
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

            // Invoke EntityMetricUpdating subscriptions
            foreach (var handler in _broker.Pub<EntityMetric>(this, "EntityMetricUpdating", model))
            {
                model = await handler.Invoke(new Message<EntityMetric>(model, this));
            }

            // Create result
            var result = new CommandResult<EntityMetric>();

            // Attempt to persist
            var reaction = await _entityRatingsStore.UpdateAsync(model);
            if (reaction != null)
            {

                // Invoke EntityMetricUpdated subscriptions
                foreach (var handler in _broker.Pub<EntityMetric>(this, "EntityMetricUpdated", reaction))
                {
                    reaction = await handler.Invoke(new Message<EntityMetric>(reaction, this));
                }

                return result.Success(reaction);

            }

            return result.Failed($"An unknown error occurred whilst attempting to update a metric");

        }

        public async Task<ICommandResult<EntityMetric>> DeleteAsync(EntityMetric model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Invoke EntityMetricDeleting subscriptions
            foreach (var handler in _broker.Pub<EntityMetric>(this, "EntityMetricDeleting", model))
            {
                model = await handler.Invoke(new Message<EntityMetric>(model, this));
            }

            var result = new CommandResult<EntityMetric>();

            var success = await _entityRatingsStore.DeleteAsync(model);
            if (success)
            {

                // Invoke EntityMetricDeleted subscriptions
                foreach (var handler in _broker.Pub<EntityMetric>(this, "EntityMetricDeleted", model))
                {
                    model = await handler.Invoke(new Message<EntityMetric>(model, this));
                }

                return result.Success(model);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the metric."));

        }

    }

}
