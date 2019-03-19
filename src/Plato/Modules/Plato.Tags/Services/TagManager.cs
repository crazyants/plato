using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Text.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Tags.Services
{
    
    public class TagManager<TModel> : ITagManager<TModel> where TModel : class, ITag
    {

        private readonly ITagStore<TModel> _tagStore;
        private readonly IAliasCreator _aliasCreator;
        private readonly IBroker _broker;

        public TagManager(
            ITagStore<TModel> tagStore,
            IAliasCreator aliasCreator,
            IBroker broker)
        {
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

            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }

            if (model.CreatedDate == null)
            {
                throw new ArgumentNullException(nameof(model.CreatedDate));
            }
            
            model.NameNormalized = model.Name.Normalize();
            model.Alias = await ParseAlias(model.Name);
  
            // Invoke TagCreating subscriptions
            foreach (var handler in _broker.Pub<TModel>(this, "TagCreating"))
            {
                model = await handler.Invoke(new Message<TModel>(model, this));
            }

            // Create result
            var result = new CommandResult<TModel>();

            // Persist to database
            var newTag = await _tagStore.CreateAsync(model);
            if (newTag != null)
            {

                // Invoke TagCreated subscriptions
                foreach (var handler in _broker.Pub<TModel>(this, "TagCreated"))
                {
                    newTag = await handler.Invoke(new Message<TModel>(newTag, this));
                }

                // Return success
                return result.Success(newTag);

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
            
            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }

            if (model.CreatedDate == null)
            {
                throw new ArgumentNullException(nameof(model.CreatedDate));
            }
            
            if (model.ModifiedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.ModifiedUserId));
            }

            if (model.ModifiedDate == null)
            {
                throw new ArgumentNullException(nameof(model.ModifiedDate));
            }
            
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
            var updatedTag = await _tagStore.UpdateAsync(model);
            if (updatedTag != null)
            {

                // Invoke TagUpdated subscriptions
                foreach (var handler in _broker.Pub<TModel>(this, "TagUpdated"))
                {
                    updatedTag = await handler.Invoke(new Message<TModel>(updatedTag, this));
                }

                // Return success
                return result.Success(updatedTag);

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
