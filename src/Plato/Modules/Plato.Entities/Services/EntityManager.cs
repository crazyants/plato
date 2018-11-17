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

        public async Task<ICommandResult<TEntity>> CreateAsync(TEntity model)
        {
            var result = new CommandResult<TEntity>();

            var user = await _contextFacade.GetAuthenticatedUserAsync();
       

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
                return result.Failed(new CommandError($"{nameof(model.Id)} cannot be greater than zero when creating an entity"));
            }

            if (model.FeatureId == 0)
            {
                return result.Failed(new CommandError($"{nameof(model.FeatureId)} must be greater than zero when creating an entity"));
            }

            if (String.IsNullOrWhiteSpace(model.Title))
            {
                return result.Failed(new CommandError($"{nameof(model.Title)} is required"));
            }

            if (String.IsNullOrWhiteSpace(model.Message))
            {
                return result.Failed(new CommandError($"{nameof(model.Message)} is required"));
            }

            // Parse Html and message abstract
            model.Html = await ParseEntityHtml(model.Message);
            model.Abstract = await ParseEntityAbstract(model.Message);
            model.Urls = await ParseEntityUrls(model.Html);
            model.Alias = await ParseEntityAlias(model.Title);
            
            // Raise creating event
            Creating?.Invoke(this, new EntityEventArgs<TEntity>(model));

            // Invoke EntityCreating subscriptions
            foreach (var handler in _broker.Pub<TEntity>(this, "EntityCreating"))
            {
                model = await handler.Invoke(new Message<TEntity>(model, this));
            }
            
            var entity = await _entityStore.CreateAsync(model);
            if (entity != null)
            {

                // Raise created event
                Created?.Invoke(this, new EntityEventArgs<TEntity>(entity));

                // Invoke EntityCreated subscriptions
                foreach (var handler in _broker.Pub<TEntity>(this, "EntityCreated"))
                {
                    entity = await handler.Invoke(new Message<TEntity>(entity, this));
                }

                // Return success
                return result.Success(entity);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an eneity"));

        }

        public async Task<ICommandResult<TEntity>> UpdateAsync(TEntity model)
        {
            var result = new CommandResult<TEntity>();

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            if (user != null)
            {
                model.ModifiedUserId = user.Id;
            }

            model.ModifiedDate = DateTime.UtcNow;

            // Validate
            if (model.Id <= 0)
            {
                return result.Failed(new CommandError($"{nameof(model.Id)} must be a valid existing entity id"));
            }

            if (String.IsNullOrWhiteSpace(model.Title))
            {
                return result.Failed(new CommandError($"{nameof(model.Title)} is required"));
            }

            if (String.IsNullOrWhiteSpace(model.Message))
            {
                return result.Failed(new CommandError($"{nameof(model.Message)} is required"));
            }

            // Parse Html and message abstract
            model.Html = await ParseEntityHtml(model.Message);
            model.Abstract = await ParseEntityAbstract(model.Message);
            model.Urls = await ParseEntityUrls(model.Html);
            model.Alias = await ParseEntityAlias(model.Title);

            // Raise Updating event
            Updating?.Invoke(this, new EntityEventArgs<TEntity>(model));

            // Invoke EntityUpdating subscriptions
            foreach (var handler in _broker.Pub<TEntity>(this, "EntityUpdating"))
            {
                model = await handler.Invoke(new Message<TEntity>(model, this));
            }

            var entity = await _entityStore.UpdateAsync(model);
            if (entity != null)
            {

                // Raise Updated event
                Updated?.Invoke(this, new EntityEventArgs<TEntity>(entity));

                // Invoke EntityUpdated subscriptions
                foreach (var handler in _broker.Pub<TEntity>(this, "EntityUpdated"))
                {
                    entity = await handler.Invoke(new Message<TEntity>(entity, this));
                }
                
                return result.Success(entity);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an eneity."));

        }

        public async Task<ICommandResult<TEntity>> DeleteAsync(TEntity model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var result = new CommandResult<TEntity>();

            var entity = await _entityStore.GetByIdAsync(model.Id);
            if (entity == null)
            {
                return result.Failed(new CommandError($"An entity with the id {model.Id} could not be found"));
            }

            // Raise Deleting event
            Deleting?.Invoke(this, new EntityEventArgs<TEntity>(entity));

            // Invoke EntityDeleting subscriptions
            foreach (var handler in _broker.Pub<TEntity>(this, new MessageOptions()
            {
                Key = "EntityDeleting"
            }, entity))
            {
                entity = await handler.Invoke(new Message<TEntity>(entity, this));
            }

            var success = await _entityStore.DeleteAsync(entity);
            if (success)
            {

                // Raise Deleted event
                Deleted?.Invoke(this, new EntityEventArgs<TEntity>(entity, true));

                // Invoke EntityDeleted subscriptions
                foreach (var handler in _broker.Pub<TEntity>(this, new MessageOptions()
                {
                    Key = "EntityDeleted"
                }, entity))
                {
                    entity = await handler.Invoke(new Message<TEntity>(entity, this));
                }

                return result.Success(entity);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an eneity."));

        }
        
        #endregion

        #region "Private Methods"

        async Task<string> ParseEntityHtml(string message)
        {
      
            foreach (var handler in _broker.Pub<string>(this, "ParseEntityHtml"))
            {
                message = await handler.Invoke(new Message<string>(message, this));
            }
            
            return message.HtmlTextulize();

        }

        async Task<string> ParseEntityAbstract(string message)
        {
            foreach (var handler in _broker.Pub<string>(this, "ParseEntityAbstract"))
            {
                message = await handler.Invoke(new Message<string>(message, this));
            }

            return message.PlainTextulize().TrimToAround(225);

        }

        async Task<string> ParseEntityAlias(string input)
        {
            foreach (var handler in _broker.Pub<string>(this, "ParseEntityAlias"))
            {
                input = await handler.Invoke(new Message<string>(input, this));
            }

            return _aliasCreator.Create(input); ;

        }

        async Task<string> ParseEntityUrls(string html)
        {

            var handled = false;
            foreach (var handler in _broker.Pub<string>(this, "ParseEntityUrls"))
            {
                handled = true;
                html = await handler.Invoke(new Message<string>(html, this));
            }
            
            return handled ? html : string.Empty;

        }
        
        #endregion

    }

}
