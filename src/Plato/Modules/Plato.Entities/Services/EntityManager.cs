using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Entities.Services
{

    public class EntityManager<TEntity> : IEntityManager<TEntity> where TEntity : class, IEntity
    {

        public event EntityEvents<TEntity>.Handler Creating;
        public event EntityEvents<TEntity>.Handler Created;
        public event EntityEvents<TEntity>.Handler Updating;
        public event EntityEvents<TEntity>.Handler Updated;
        public event EntityEvents<TEntity>.Handler Deleting;
        public event EntityEvents<TEntity>.Handler Deleted;

        #region "Constructor"

        private readonly IBroker _broker;
        private readonly IEntityStore<TEntity> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly IAliasCreator _aliasCreator;

        public EntityManager(
            IEntityStore<TEntity> entityStore,
            IBroker broker,
            IContextFacade contextFacade,
            IAliasCreator aliasCreator)
        {
            _entityStore = entityStore;
            _broker = broker;
            _contextFacade = contextFacade;
            _aliasCreator = aliasCreator;
        }

        #endregion

        #region "Implementation"

        public async Task<IActivityResult<TEntity>> CreateAsync(TEntity model)
        {
            var result = new ActivityResult<TEntity>();

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            var feature = await _contextFacade.GetFeatureByAreaAsync();

            // Set entities featureId based on current feature
            if (feature != null)
            {
                model.FeatureId = feature.Id;
            }

            if (user != null)
            {
                model.CreatedUserId = user.Id;
                model.ModifiedUserId = user.Id;
            }

            model.CreatedDate = DateTime.UtcNow;
            model.ModifiedDate = DateTime.UtcNow;

            // Validate
            if (model.Id > 0)
            {
                return result.Failed(new ActivityError($"{nameof(model.Id)} cannot be greater than zero when creating an entity"));
            }

            if (model.FeatureId == 0)
            {
                return result.Failed(new ActivityError($"{nameof(model.FeatureId)} must be greater than zero when creating an entity"));
            }

            if (String.IsNullOrWhiteSpace(model.Title))
            {
                return result.Failed(new ActivityError($"{nameof(model.Title)} is required"));
            }

            if (String.IsNullOrWhiteSpace(model.Message))
            {
                return result.Failed(new ActivityError($"{nameof(model.Message)} is required"));
            }

            // Parse Html and message abstract
            model.Html = await ParseMarkdown(model.Message);
            model.Abstract = await ParseAbstract(model.Message);
            model.Alias = await ParseAlias(model.Title);

            // Raise creating event
            Creating?.Invoke(this, new EntityEventArgs<TEntity>(model));

            // Publish EntityCreating event
            await _broker.Pub<TEntity>(this, new MessageOptions()
            {
                Key = "EntityCreating"
            }, model);

            var entity = await _entityStore.CreateAsync(model);
            if (entity != null)
            {

                // Raise created event
                Created?.Invoke(this, new EntityEventArgs<TEntity>(entity));

                // Publish EntityCreated event
                await _broker.Pub<TEntity>(this, new MessageOptions()
                {
                    Key = "EntityCreated"
                }, model);

                // Return success
                return result.Success(entity);
            }

            return result.Failed(new ActivityError("An unknown error occurred whilst attempting to create an eneity"));

        }

        public async Task<IActivityResult<TEntity>> UpdateAsync(TEntity model)
        {
            var result = new ActivityResult<TEntity>();

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            if (user != null)
            {
                model.ModifiedUserId = user.Id;
            }

            model.ModifiedDate = DateTime.UtcNow;

            // Validate
            if (model.Id <= 0)
            {
                return result.Failed(new ActivityError($"{nameof(model.Id)} must be a valid existing entity id"));
            }

            if (String.IsNullOrWhiteSpace(model.Title))
            {
                return result.Failed(new ActivityError($"{nameof(model.Title)} is required"));
            }

            if (String.IsNullOrWhiteSpace(model.Message))
            {
                return result.Failed(new ActivityError($"{nameof(model.Message)} is required"));
            }

            // Parse Html and message abstract
            model.Html = await ParseMarkdown(model.Message);
            model.Abstract = await ParseAbstract(model.Message);
            model.Alias = await ParseAlias(model.Title);

            // Raise Updating event
            Updating?.Invoke(this, new EntityEventArgs<TEntity>(model));

            // Publish EntityUpdating event
            await _broker.Pub<TEntity>(this, new MessageOptions()
            {
                Key = "EntityUpdating"
            }, model);

            var entity = await _entityStore.UpdateAsync(model);
            if (entity != null)
            {

                // Raise Updated event
                Updated?.Invoke(this, new EntityEventArgs<TEntity>(entity));

                // Publish EntityUpdated event
                await _broker.Pub<TEntity>(this, new MessageOptions()
                {
                    Key = "EntityUpdated"
                }, model);

                return result.Success(entity);
            }

            return result.Failed(new ActivityError("An unknown error occurred whilst attempting to create an eneity."));

        }

        public async Task<IActivityResult<TEntity>> DeleteAsync(TEntity model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var result = new ActivityResult<TEntity>();

            var entity = await _entityStore.GetByIdAsync(model.Id);
            if (entity == null)
            {
                return result.Failed(new ActivityError($"An entity with the id {model.Id} could not be found"));
            }

            // Raise Deleting event
            Deleting?.Invoke(this, new EntityEventArgs<TEntity>(entity));

            // Publish EntityDeleting event
            await _broker.Pub<TEntity>(this, new MessageOptions()
            {
                Key = "EntityDeleting"
            }, entity);

            var success = await _entityStore.DeleteAsync(entity);
            if (success)
            {

                // Raise Deleted event
                Deleted?.Invoke(this, new EntityEventArgs<TEntity>(entity, true));

                // Publish EntityDeleted event
                await _broker.Pub<TEntity>(this, new MessageOptions()
                {
                    Key = "EntityDeleted"
                }, entity);

                return result.Success(entity);
            }

            return result.Failed(new ActivityError("An unknown error occurred whilst attempting to create an eneity."));

        }


        #endregion

        #region "Private Methods"

        private async Task<string> ParseMarkdown(string message)
        {

            foreach (var handler in await _broker.Pub<string>(this, new MessageOptions()
            {
                Key = "ParseMarkdown"
            }, message))
            {
                return await handler.Invoke(new Message<string>(message, this));
            }

            return message;

        }

        private async Task<string> ParseAbstract(string message)
        {

            foreach (var handler in await _broker.Pub<string>(this, new MessageOptions()
            {
                Key = "ParseAbstract"
            }, message))
            {
                return await handler.Invoke(new Message<string>(message, this));
            }

            return message.PlainTextulize().TrimToAround(500);

        }

        private async Task<string> ParseAlias(string input)
        {

            foreach (var handler in await _broker.Pub<string>(this, new MessageOptions()
            {
                Key = "ParseEntityAlias"
            }, input))
            {
                return await handler.Invoke(new Message<string>(input, this));
            }

            // No subscription found, use default alias creator
            return _aliasCreator.Create(input);

        }


        #endregion

    }

}
