using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Text.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Tags.Services
{
    
    public class TagManager<TModel> : ITagManager<TModel> where TModel : class, ITag
    {

        private readonly ITagStore<TModel> _tagStore;
        private readonly IContextFacade _contextFacade;
        private readonly IAliasCreator _aliasCreator;
        private readonly IBroker _broker;

        public TagManager(
            ITagStore<TModel> tagStore,
            IContextFacade contextFacade,
            IAliasCreator aliasCreator,
            IBroker broker)
        {
            _contextFacade = contextFacade;
            _aliasCreator = aliasCreator;
            _tagStore = tagStore;
            _broker = broker;
        }

        #region "Implementation"

        public async Task<ICommandResult<TModel>> CreateAsync(TModel model)
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
            
            model.NameNormalized = model.Name.Normalize();
            model.Alias = await ParseAlias(model.Name);
            model.CreatedUserId = user?.Id ?? 0;
            model.CreatedDate = DateTime.UtcNow;
            

            // Invoke TagCreating subscriptions
            foreach (var handler in _broker.Pub<TModel>(this, "TagCreating"))
            {
                model = await handler.Invoke(new Message<TModel>(model, this));
            }

            // Create result
            var result = new CommandResult<TModel>();

            // Persist to database
            var newEntityTag = await _tagStore.CreateAsync(model);
            if (newEntityTag != null)
            {

                // Invoke TagCreated subscriptions
                foreach (var handler in _broker.Pub<TModel>(this, "TagCreated"))
                {
                    newEntityTag = await handler.Invoke(new Message<TModel>(newEntityTag, this));
                }

                // Return success
                return result.Success(newEntityTag);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an entity / tag relatinship"));

        }

        public async Task<ICommandResult<TModel>> UpdateAsync(TModel model)
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

            model.NameNormalized = model.Name.Normalize();
            model.Alias = await ParseAlias(model.Name);

            // Invoke TagUpdating subscriptions
            foreach (var handler in _broker.Pub<TModel>(this, "TagUpdating"))
            {
                model = await handler.Invoke(new Message<TModel>(model, this));
            }

            // Create result
            var result = new CommandResult<TModel>();

            // Persist to database
            var updatedEntityTag = await _tagStore.UpdateAsync(model);
            if (updatedEntityTag != null)
            {

                // Invoke TagUpdated subscriptions
                foreach (var handler in _broker.Pub<TModel>(this, "TagUpdated"))
                {
                    updatedEntityTag = await handler.Invoke(new Message<TModel>(updatedEntityTag, this));
                }

                // Return success
                return result.Success(updatedEntityTag);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an entity / tag relatinship"));

        }

        public async Task<ICommandResult<TModel>> DeleteAsync(TModel model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Invoke TagDeleting subscriptions
            foreach (var handler in _broker.Pub<TModel>(this, "TagDeleting"))
            {
                model = await handler.Invoke(new Message<TModel>(model, this));
            }

            var result = new CommandResult<TModel>();
            if (await _tagStore.DeleteAsync(model))
            {

                // Invoke TagDeleted subscriptions
                foreach (var handler in _broker.Pub<TModel>(this, "TagDeleted"))
                {
                    model = await handler.Invoke(new Message<TModel>(model, this));
                }

                // Return success
                return result.Success();

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the entity / tag relationship"));
            
        }

        #endregion

        #region "Private Methods"

        private async Task<string> ParseAlias(string input)
        {

            var handled = false;
            foreach (var handler in _broker.Pub<string>(this, "ParseTagAlias"))
            {
                handled = true;
                input = await handler.Invoke(new Message<string>(input, this));
            }

            // No subscription found, use default alias creator
            return handled ? input : _aliasCreator.Create(input);

        }

        #endregion

    }

}
