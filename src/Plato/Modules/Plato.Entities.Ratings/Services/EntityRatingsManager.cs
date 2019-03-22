using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Ratings.Models;
using Plato.Entities.Ratings.Stores;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Entities.Ratings.Services
{

    public class EntityRatingsManager : IEntityRatingsManager<EntityRating>
    {

        private readonly IEntityRatingsStore<EntityRating> _entityRatingsStore;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IBroker _broker;

        public EntityRatingsManager(
            IEntityRatingsStore<EntityRating> entityRatingsStore,
            IEntityStore<Entity> entityStore,
            IBroker broker)
        {
            _entityRatingsStore = entityRatingsStore;
            _entityStore = entityStore;
            _broker = broker;
        }

        public async Task<ICommandResult<EntityRating>> CreateAsync(EntityRating model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            // Get entity
            var entity = await _entityStore.GetByIdAsync(model.EntityId);
            if (entity == null)
            {
                throw new Exception("The entity no longer exists!");
            }
            
            model.FeatureId = entity.FeatureId;
            model.CreatedDate = DateTime.UtcNow;

            // Invoke EntityRatingCreating subscriptions
            foreach (var handler in _broker.Pub<EntityRating>(this, "EntityRatingCreating", model))
            {
                model = await handler.Invoke(new Message<EntityRating>(model, this));
            }

            // Create result
            var result = new CommandResult<EntityRating>();

            // Attempt to persist
            var reaction = await _entityRatingsStore.CreateAsync(model);
            if (reaction != null)
            {

                // Invoke EntityRatingCreated subscriptions
                foreach (var handler in _broker.Pub<EntityRating>(this, "EntityRatingCreated", reaction))
                {
                    reaction = await handler.Invoke(new Message<EntityRating>(reaction, this));
                }

                return result.Success(reaction);

            }

            return result.Failed($"An unknown error occurred whilst attempting to create a reaction");

        }

        public async Task<ICommandResult<EntityRating>> UpdateAsync(EntityRating model)
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

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }
            
            // Get entity
            var entity = await _entityStore.GetByIdAsync(model.EntityId);
            if (entity == null)
            {
                throw new Exception("The entity no longer exists!");
            }
            
            model.FeatureId = entity.FeatureId;

            // Invoke EntityRatingUpdating subscriptions
            foreach (var handler in _broker.Pub<EntityRating>(this, "EntityRatingUpdating", model))
            {
                model = await handler.Invoke(new Message<EntityRating>(model, this));
            }

            // Create result
            var result = new CommandResult<EntityRating>();

            // Attempt to persist
            var reaction = await _entityRatingsStore.UpdateAsync(model);
            if (reaction != null)
            {

                // Invoke EntityRatingUpdated subscriptions
                foreach (var handler in _broker.Pub<EntityRating>(this, "EntityRatingUpdated", reaction))
                {
                    reaction = await handler.Invoke(new Message<EntityRating>(reaction, this));
                }

                return result.Success(reaction);

            }

            return result.Failed($"An unknown error occurred whilst attempting to update a reaction");

        }

        public async Task<ICommandResult<EntityRating>> DeleteAsync(EntityRating model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Invoke EntityRatingDeleting subscriptions
            foreach (var handler in _broker.Pub<EntityRating>(this, "EntityRatingDeleting", model))
            {
                model = await handler.Invoke(new Message<EntityRating>(model, this));
            }

            var result = new CommandResult<EntityRating>();

            var success = await _entityRatingsStore.DeleteAsync(model);
            if (success)
            {

                // Invoke EntityRatingDeleted subscriptions
                foreach (var handler in _broker.Pub<EntityRating>(this, "EntityRatingDeleted", model))
                {
                    model = await handler.Invoke(new Message<EntityRating>(model, this));
                }

                return result.Success(model);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the reoaction."));

        }

    }

}
