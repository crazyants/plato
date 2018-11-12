using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Mentions.Models;
using Plato.Mentions.Stores;

namespace Plato.Mentions.Services
{
    
    public class EntityMentionsManager : IEntityMentionsManager<EntityMention>
    {

        private readonly IEntityMentionsStore<EntityMention> _entityMentionsStore;
        private readonly IContextFacade _contextFacade;
        private readonly IBroker _broker;

        public EntityMentionsManager(
            IEntityMentionsStore<EntityMention> entityMentionsStore,
            IContextFacade contextFacade,
            IBroker broker)
        {
            _entityMentionsStore = entityMentionsStore;
            _contextFacade = contextFacade;
            _broker = broker;
        }

        public async Task<ICommandResult<EntityMention>> CreateAsync(EntityMention model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.EntityId <= 0 && model.EntityReplyId <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(model.EntityId)} or {nameof(model.EntityReplyId)} must be greater than zero.");
            }

            if (model.UserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.UserId));
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update
            model.CreatedUserId = user?.Id ?? 0;
            model.CreatedDate = DateTime.UtcNow;
            
            // Invoke EntityMentionCreating subscriptions
            foreach (var handler in _broker.Pub<EntityMention>(this, new MessageOptions()
            {
                Key = "EntityMentionCreating"
            }, model))
            {
                model = await handler.Invoke(new Message<EntityMention>(model, this));
            }

            // Create result
            var result = new CommandResult<EntityMention>();

            // Persist to database
            var newEntityMention = await _entityMentionsStore.CreateAsync(model);
            if (newEntityMention != null)
            {

                // Invoke EntityMentionCreated subscriptions
                foreach (var handler in _broker.Pub<EntityMention>(this, new MessageOptions()
                {
                    Key = "EntityMentionCreated"
                }, newEntityMention))
                {
                    newEntityMention = await handler.Invoke(new Message<EntityMention>(newEntityMention, this));
                }

                // Return success
                return result.Success(newEntityMention);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an entity / label relatinship"));

        }

        public async Task<ICommandResult<EntityMention>> UpdateAsync(EntityMention model)
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

            if (model.EntityId <= 0 && model.EntityReplyId <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(model.EntityId)} or {nameof(model.EntityReplyId)} must be greater than zero.");
            }

            if (model.UserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.UserId));
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Invoke EntityMentionUpdating subscriptions
            foreach (var handler in _broker.Pub<EntityMention>(this, new MessageOptions()
            {
                Key = "EntityMentionUpdating"
            }, model))
            {
                model = await handler.Invoke(new Message<EntityMention>(model, this));
            }

            // Create result
            var result = new CommandResult<EntityMention>();

            // Persist to database
            var updatedEntityMention = await _entityMentionsStore.UpdateAsync(model);
            if (updatedEntityMention != null)
            {

                // Invoke EntityMentionUpdated subscriptions
                foreach (var handler in _broker.Pub<EntityMention>(this, new MessageOptions()
                {
                    Key = "EntityMentionUpdated"
                }, updatedEntityMention))
                {
                    updatedEntityMention = await handler.Invoke(new Message<EntityMention>(updatedEntityMention, this));
                }

                // Return success
                return result.Success(updatedEntityMention);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an entity / label relatinship"));

        }

        public async Task<ICommandResult<EntityMention>> DeleteAsync(EntityMention model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Invoke EntityMentionDeleting subscriptions
            foreach (var handler in _broker.Pub<EntityMention>(this, new MessageOptions()
            {
                Key = "EntityMentionDeleting"
            }, model))
            {
                model = await handler.Invoke(new Message<EntityMention>(model, this));
            }

            var result = new CommandResult<EntityMention>();
            if (await _entityMentionsStore.DeleteAsync(model))
            {

                // Invoke EntityLabelDeleted subscriptions
                foreach (var handler in _broker.Pub<EntityMention>(this, new MessageOptions()
                {
                    Key = "EntityMentionDeleted"
                }, model))
                {
                    model = await handler.Invoke(new Message<EntityMention>(model, this));
                }

                // Return success
                return result.Success();

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the entity / label relationship"));
            
        }

    }

}
