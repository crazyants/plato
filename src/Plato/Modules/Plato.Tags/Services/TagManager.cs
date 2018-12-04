using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Tags.Services
{
    
    public class TagManager : ITagManager<Tag>
    {

        private readonly ITagStore<Tag> _tagStore;
        private readonly IContextFacade _contextFacade;
        private readonly IBroker _broker;

        public TagManager(
            ITagStore<Tag> tagStore,
            IContextFacade contextFacade,
            IBroker broker)
        {
            _tagStore = tagStore;
            _broker = broker;
            _contextFacade = contextFacade;
        }

        public async Task<ICommandResult<Tag>> CreateAsync(Tag model)
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

            if (model.FeatureId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.FeatureId));
            }
            
            if (String.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentOutOfRangeException(nameof(model.Name));
            }
        
            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update
            model.CreatedUserId = user?.Id ?? 0;
            model.CreatedDate = DateTime.UtcNow;
          
            // Invoke TagCreating subscriptions
            foreach (var handler in _broker.Pub<Tag>(this, "TagCreating"))
            {
                model = await handler.Invoke(new Message<Tag>(model, this));
            }

            // Create result
            var result = new CommandResult<Tag>();

            // Persist to database
            var newEntityTag = await _tagStore.CreateAsync(model);
            if (newEntityTag != null)
            {

                // Invoke TagCreated subscriptions
                foreach (var handler in _broker.Pub<Tag>(this, "TagCreated"))
                {
                    newEntityTag = await handler.Invoke(new Message<Tag>(newEntityTag, this));
                }

                // Return success
                return result.Success(newEntityTag);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an entity / tag relatinship"));

        }

        public async Task<ICommandResult<Tag>> UpdateAsync(Tag model)
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

            if (model.FeatureId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.FeatureId));
            }
            
            if (String.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentOutOfRangeException(nameof(model.Name));
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();


            // Invoke TagUpdating subscriptions
            foreach (var handler in _broker.Pub<Tag>(this, "TagUpdating"))
            {
                model = await handler.Invoke(new Message<Tag>(model, this));
            }

            // Create result
            var result = new CommandResult<Tag>();

            // Persist to database
            var updatedEntityTag = await _tagStore.UpdateAsync(model);
            if (updatedEntityTag != null)
            {

                // Invoke TagUpdated subscriptions
                foreach (var handler in _broker.Pub<Tag>(this, "TagUpdated"))
                {
                    updatedEntityTag = await handler.Invoke(new Message<Tag>(updatedEntityTag, this));
                }

                // Return success
                return result.Success(updatedEntityTag);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an entity / tag relatinship"));

        }

        public async Task<ICommandResult<Tag>> DeleteAsync(Tag model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Invoke TagDeleting subscriptions
            foreach (var handler in _broker.Pub<Tag>(this, "TagDeleting"))
            {
                model = await handler.Invoke(new Message<Tag>(model, this));
            }

            var result = new CommandResult<Tag>();
            if (await _tagStore.DeleteAsync(model))
            {

                // Invoke TagDeleted subscriptions
                foreach (var handler in _broker.Pub<Tag>(this, "TagDeleted"))
                {
                    model = await handler.Invoke(new Message<Tag>(model, this));
                }

                // Return success
                return result.Success();

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the entity / tag relationship"));
            
        }

    }

}
