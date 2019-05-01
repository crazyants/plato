using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;

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

  
        public EntityManager(
            IEntityStore<TEntity> entityStore,
            IContextFacade contextFacade,
            IBroker broker)
        {
            _entityStore = entityStore;
            _contextFacade = contextFacade;
            _broker = broker;
        }

        #endregion

        #region "Implementation"

        public async Task<ICommandResult<TEntity>> CreateAsync(TEntity model)
        {
            var result = new CommandResult<TEntity>();

            // Validate
            if (model.Id > 0)
            {
                return result.Failed(new CommandError($"{nameof(model.Id)} cannot be greater than zero when creating an entity"));
            }

            if (model.FeatureId == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.FeatureId));
            }
            
            if (String.IsNullOrWhiteSpace(model.Title))
            {
                throw new ArgumentNullException(nameof(model.Title));
            }

            if (String.IsNullOrWhiteSpace(model.Message))
            {
                throw new ArgumentNullException(nameof(model.Message));
            }
            
            if (model.CreatedDate == null)
            {
                throw new ArgumentNullException(nameof(model.CreatedDate));
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

            // Validate
            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.FeatureId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.FeatureId));
            }
            
            if (String.IsNullOrWhiteSpace(model.Title))
            {
                throw new ArgumentNullException(nameof(model.Title));
            }

            if (String.IsNullOrWhiteSpace(model.Message))
            {
                throw new ArgumentNullException(nameof(model.Message));
            }

            if (model.ModifiedDate == null)
            {
                throw new ArgumentNullException(nameof(model.ModifiedDate));
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

        public async Task<ICommandResult<TEntity>> Move(TEntity model, MoveDirection direction)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Our result
            var result = new CommandResult<TEntity>();

            // All categories for supplied category feature
            var entities = await _entityStore.GetByFeatureIdAsync(model.FeatureId);
            if (entities == null)
            {
                return result.Failed($"No entities were found matching FeatureId '{model.FeatureId}'");
            }
            
            var currentSortOrder = model.SortOrder;
            List<TEntity> children = null;
            switch (direction)
            {

                case MoveDirection.Up:

                    // Find entity above the supplied entity
                    TEntity above = null;

                    foreach (var entity in entities.Where(c => c.ParentId == model.ParentId))
                    {
                        if (entity.SortOrder < currentSortOrder)
                        {
                            above = (TEntity)entity;
                        }
                    }

                    // Swap sort orders
                    if (above != null)
                    {

                        // Update source
                        await UpdateSortOrder(model, above.SortOrder);

                        // Update entity we are swapping with modified detailed
                        above.ModifiedUserId = model.ModifiedUserId;
                        above.ModifiedDate = model.ModifiedDate;
                        
                        // Update target
                        await UpdateSortOrder(above, currentSortOrder);

                    }

                    break;

                case MoveDirection.Down:

                    // Find category below the supplied category
                    TEntity below = null;
                    children = entities
                        .Where(c => c.ParentId == model.ParentId)
                        .ToList();
                    for (var i = children.Count - 1; i >= 0; i--)
                    {
                        if (children[i].SortOrder > currentSortOrder)
                        {
                            below = (TEntity)children[i];
                        }
                    }

                    // Swap sort orders
                    if (below != null)
                    {

                        // Update source
                        await UpdateSortOrder(model, below.SortOrder);
                        
                        // Update entity we are swapping with modified detailed
                        below.ModifiedUserId = model.ModifiedUserId;
                        below.ModifiedDate = model.ModifiedDate;
                        
                        // Update target
                        await UpdateSortOrder(below, currentSortOrder);

                    }

                    break;

                case MoveDirection.ToTop:

                    // Find entity at the top of the current level
                    TEntity top = null;
                    children = entities.Where(c => c.ParentId == model.ParentId).ToList();
                    for (var i = children.Count - 1; i >= 0; i--)
                    {
                        top = (TEntity)children[i];
                    }
                  
                    if (top != null && top.Id != model.Id)
                    {
                        await UpdateSortOrder(model, top.SortOrder - 1);
                    }

                    break;

                case MoveDirection.ToBottom:

                    // Find entity at the bottom of the current level
                    TEntity bottom = null;
                    foreach (var entity in entities.Where(c => c.ParentId == model.ParentId))
                    {
                        bottom = (TEntity)entity;
                    }
                    
                    if (bottom != null && bottom.Id != model.Id)
                    {
                        await UpdateSortOrder(model, bottom.SortOrder + 1);
                    }

                    break;
            }

            return result.Success();

        }

        #endregion

        #region "Private Methods"

        async Task<ICommandResult<TEntity>> UpdateSortOrder(TEntity model, int sortOrder)
        {
            model.SortOrder = sortOrder;
            return await UpdateAsync(model);
        }

        async Task<string> ParseEntityHtml(string message)
        {
            
            foreach (var handler in _broker.Pub<string>(this, "ParseEntityHtml"))
            {
                message = await handler.Invoke(new Message<string>(message, this));
            }
            
            return message;

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
            var handled = false;
            foreach (var handler in _broker.Pub<string>(this, "ParseEntityAlias"))
            {
                handled = true;
                input = await handler.Invoke(new Message<string>(input, this));
            }

            return handled ? input : string.Empty;

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
