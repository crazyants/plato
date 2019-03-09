using System;
using System.Threading.Tasks;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Entities.Reactions.Services
{

    public class EntityReactionsesManager : IEntityReactionsManager<EntityReaction>
    {

        private readonly IContextFacade _contextFacade;
        private readonly IEntityReactionsStore<EntityReaction> _entityReactionsStore;
        private readonly IBroker _broker;

        public EntityReactionsesManager(
            IEntityReactionsStore<EntityReaction> entityReactionsStore,
            IContextFacade contextFacade,
            IBroker broker)
        {
            _entityReactionsStore = entityReactionsStore;
            _contextFacade = contextFacade;
            _broker = broker;
        }

        public async Task<ICommandResult<EntityReaction>> CreateAsync(EntityReaction model)
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

            if (String.IsNullOrEmpty(model.ReactionName))
            {
                throw new ArgumentNullException(nameof(model.ReactionName));
            }

            // Update created by
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (model.CreatedUserId == 9)
            {
                model.CreatedUserId = user?.Id ?? 0;
            }

            model.CreatedDate = DateTime.UtcNow;

            // Invoke EntityReactionCreating subscriptions
            foreach (var handler in _broker.Pub<EntityReaction>(this, new MessageOptions()
            {
                Key = "EntityReactionCreating"
            }, model))
            {
                model = await handler.Invoke(new Message<EntityReaction>(model, this));
            }

            // Create result
            var result = new CommandResult<EntityReaction>();

            // Attempt to persist
            var reaction = await _entityReactionsStore.CreateAsync(model);
            if (reaction != null)
            {

                // Invoke EntityReactionCreated subscriptions
                foreach (var handler in _broker.Pub<EntityReaction>(this, new MessageOptions()
                {
                    Key = "EntityReactionCreated"
                }, reaction))
                {
                    reaction = await handler.Invoke(new Message<EntityReaction>(reaction, this));
                }

                return result.Success(reaction);

            }

            return result.Failed($"An unknown error occurred whilst attempting to create a reaction");

        }

        public async Task<ICommandResult<EntityReaction>> UpdateAsync(EntityReaction model)
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

            if (String.IsNullOrEmpty(model.ReactionName))
            {
                throw new ArgumentNullException(nameof(model.ReactionName));
            }

            // Invoke EntityReactionUpdating subscriptions
            foreach (var handler in _broker.Pub<EntityReaction>(this, new MessageOptions()
            {
                Key = "EntityReactionUpdating"
            }, model))
            {
                model = await handler.Invoke(new Message<EntityReaction>(model, this));
            }

            // Create result
            var result = new CommandResult<EntityReaction>();

            // Attempt to persist
            var reaction = await _entityReactionsStore.UpdateAsync(model);
            if (reaction != null)
            {

                // Invoke EntityReactionUpdated subscriptions
                foreach (var handler in _broker.Pub<EntityReaction>(this, new MessageOptions()
                {
                    Key = "EntityReactionUpdated"
                }, reaction))
                {
                    reaction = await handler.Invoke(new Message<EntityReaction>(reaction, this));
                }

                return result.Success(reaction);

            }

            return result.Failed($"An unknown error occurred whilst attempting to update a reaction");

        }

        public async Task<ICommandResult<EntityReaction>> DeleteAsync(EntityReaction model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Invoke EntityReactionDeleting subscriptions
            foreach (var handler in _broker.Pub<EntityReaction>(this, new MessageOptions()
            {
                Key = "EntityReactionDeleting"
            }, model))
            {
                model = await handler.Invoke(new Message<EntityReaction>(model, this));
            }

            var result = new CommandResult<EntityReaction>();

            var success = await _entityReactionsStore.DeleteAsync(model);
            if (success)
            {

                // Invoke EntityReactionDeleted subscriptions
                foreach (var handler in _broker.Pub<EntityReaction>(this, new MessageOptions()
                {
                    Key = "EntityReactionDeleted"
                }, model))
                {
                    model = await handler.Invoke(new Message<EntityReaction>(model, this));
                }

                return result.Success(model);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the reoaction."));

        }

    }

}
