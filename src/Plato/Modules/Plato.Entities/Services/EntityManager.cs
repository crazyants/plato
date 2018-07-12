using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Entities.Services
{
    
    public class EntityManager2<TModel> : IEntityManager<TModel> where TModel : class, IEntity
    {

        public event EntityEvents<TModel>.Handler Creating;
        public event EntityEvents<TModel>.Handler Created;
        public event EntityEvents<TModel>.Handler Updating;
        public event EntityEvents<TModel>.Handler Updated;
        public event EntityEvents<TModel>.Handler Deleting;
        public event EntityEvents<TModel>.Handler Deleted;
   
        #region "Constructor"

        private readonly IBroker _broker;
        private readonly IEntityStore<TModel> _entityStore;
        private readonly IContextFacade _contextFacade;

        public EntityManager2(
            IEntityStore<TModel> entityStore,
            IBroker broker,
            IContextFacade contextFacade)
        {
            _entityStore = entityStore;
            _broker = broker;
            _contextFacade = contextFacade;
        }

        #endregion

        #region "Implementation"

        public async Task<IActivityResult<TModel>> CreateAsync(TModel model)
        {
            var result = new ActivityResult<TModel>();

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            var feature = await _contextFacade.GetCurrentFeatureAsync();

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
                return result.Failed(new EntityError($"{nameof(model.Id)} cannot be greater than zero when creating an entity"));
            }

            if (model.FeatureId == 0)
            {
                return result.Failed(new EntityError($"{nameof(model.FeatureId)} must be greater than zero when creating an entity"));
            }

            if (String.IsNullOrWhiteSpace(model.Title))
            {
                return result.Failed(new EntityError($"{nameof(model.Title)} is required"));
            }

            if (String.IsNullOrWhiteSpace(model.Message))
            {
                return result.Failed(new EntityError($"{nameof(model.Message)} is required"));
            }

            // Parse Html and message abstract
            model.Html = await ParseMarkdown(model.Message);
            model.Abstract = await ParseAbstract(model.Message);

            // Raise creating event
            Creating?.Invoke(this, new EntityEventArgs<TModel>(model));

            // Publish EntityCreating event
            await _broker.Pub<TModel>(this, new MessageOptions()
            {
                Key = "EntityCreating"
            }, model);

            var entity = await _entityStore.CreateAsync(model);
            if (entity != null)
            {

                // Raise created event
                Created?.Invoke(this, new EntityEventArgs<TModel>(entity));

                // Publish EntityCreated event
                await _broker.Pub<TModel>(this, new MessageOptions()
                {
                    Key = "EntityCreated"
                }, model);

                // Return success
                return result.Success(entity);
            }

            return result.Failed(new EntityError("An unknown error occurred whilst attempting to create an eneity"));

        }

        public async Task<IActivityResult<TModel>> UpdateAsync(TModel model)
        {
            var result = new ActivityResult<TModel>();

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            if (user != null)
            {
                model.ModifiedUserId = user.Id;
            }

            model.ModifiedDate = DateTime.UtcNow;

            // Validate
            if (model.Id <= 0)
            {
                return result.Failed(new EntityError($"{nameof(model.Id)} must be a valid existing entity id"));
            }

            if (String.IsNullOrWhiteSpace(model.Title))
            {
                return result.Failed(new EntityError($"{nameof(model.Title)} is required"));
            }

            if (String.IsNullOrWhiteSpace(model.Message))
            {
                return result.Failed(new EntityError($"{nameof(model.Message)} is required"));
            }

            // Parse Html and message abstract
            model.Html = await ParseMarkdown(model.Message);
            model.Abstract = await ParseAbstract(model.Message);

            // Raise Updating event
            Updating?.Invoke(this, new EntityEventArgs<TModel>(model));

            // Publish EntityUpdating event
            await _broker.Pub<TModel>(this, new MessageOptions()
            {
                Key = "EntityUpdating"
            }, model);

            var entity = await _entityStore.UpdateAsync(model);
            if (entity != null)
            {

                // Raise Updated event
                Updated?.Invoke(this, new EntityEventArgs<TModel>(entity));

                // Publish EntityUpdated event
                await _broker.Pub<TModel>(this, new MessageOptions()
                {
                    Key = "EntityUpdated"
                }, model);

                return result.Success(entity);
            }

            return result.Failed(new EntityError("An unknown error occurred whilst attempting to create an eneity."));

        }

        public async Task<IActivityResult<TModel>> DeleteAsync(int id)
        {

            var result = new ActivityResult<TModel>();

            var entity = await _entityStore.GetByIdAsync(id);
            if (entity == null)
            {
                return result.Failed(new EntityError($"An entity is the id {id} could not be found"));
            }

            // Raise Deleting event
            Deleting?.Invoke(this, new EntityEventArgs<TModel>(entity));

            // Publish EntityDeleting event
            await _broker.Pub<TModel>(this, new MessageOptions()
            {
                Key = "EntityDeleting"
            }, entity);

            var success = await _entityStore.DeleteAsync(entity);
            if (success)
            {

                // Raise Deleted event
                Deleted?.Invoke(this, new EntityEventArgs<TModel>(entity, true));

                // Publish EntityDeleted event
                await _broker.Pub<TModel>(this, new MessageOptions()
                {
                    Key = "EntityDeleted"
                }, entity);

                return result.Success(entity);
            }

            return result.Failed(new EntityError("An unknown error occurred whilst attempting to create an eneity."));

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

            return message.StripHtml().TrimToAround(500);

        }

        #endregion

    }



    //public class EntityManager : IEntityManager<Entity>
    //{

    //    public event EntityEvents.Handler Creating;
    //    public event EntityEvents.Handler Created;
    //    public event EntityEvents.Handler Updating;
    //    public event EntityEvents.Handler Updated;
    //    public event EntityEvents.Handler Deleting;
    //    public event EntityEvents.Handler Deleted;
   
    //    #region "Constructor"

    //    private readonly IBroker _broker;
    //    private readonly IEntityStore<Entity> _entityStore;
    //    private readonly IContextFacade _contextFacade;

    //    public EntityManager(
    //        IEntityStore<Entity> entityStore,
    //        IBroker broker, 
    //        IContextFacade contextFacade)
    //    {
    //        _entityStore = entityStore;
    //        _broker = broker;
    //        _contextFacade = contextFacade;
    //    }

    //    #endregion

    //    #region "Implementation"

    //    public async Task<IActivityResult<Entity>> CreateAsync(Entity model)
    //    {

    //        var result = new ActivityResult<Entity>();
            
    //        var user = await _contextFacade.GetAuthenticatedUserAsync();
    //        var feature = await _contextFacade.GetCurrentFeatureAsync();

    //        // Set entities featureId based on current feature
    //        if (feature != null)
    //        {
    //            model.FeatureId = feature.Id;
    //        }
            
    //        if (user != null)
    //        {
    //            model.CreatedUserId = user.Id;
    //            model.ModifiedUserId = user.Id;
    //        }

    //        model.CreatedDate = DateTime.UtcNow;
    //        model.ModifiedDate = DateTime.UtcNow;

    //        // Validate
    //        if (model.Id > 0)
    //        {
    //            return result.Failed(new EntityError($"{nameof(model.Id)} cannot be greater than zero when creating an entity"));
    //        }

    //        if (model.FeatureId == 0)
    //        {
    //            return result.Failed(new EntityError($"{nameof(model.FeatureId)} must be greater than zero when creating an entity"));
    //        }

    //        if (String.IsNullOrWhiteSpace(model.Title))
    //        {
    //            return result.Failed(new EntityError($"{nameof(model.Title)} is required"));
    //        }

    //        if (String.IsNullOrWhiteSpace(model.Message))
    //        {
    //            return result.Failed(new EntityError($"{nameof(model.Message)} is required"));
    //        }

    //        // Parse Html and message abstract
    //        model.Html = await ParseMarkdown(model.Message);
    //        model.Abstract = await ParseAbstract(model.Message);
            
    //        // Raise creating event
    //        Creating?.Invoke(this, new EntityEventArgs(model));

    //        // Publish EntityCreating event
    //        await _broker.Pub<Entity>(this, new MessageOptions()
    //        {
    //            Key = "EntityCreating"
    //        }, model);
            
    //        var entity = await _entityStore.CreateAsync(model);
    //        if (entity != null)
    //        {

    //            // Raise created event
    //            Created?.Invoke(this, new EntityEventArgs(entity));

    //            // Publish EntityCreated event
    //            await _broker.Pub<Entity>(this, new MessageOptions()
    //            {
    //                Key = "EntityCreated"
    //            }, model);

    //            // Return success
    //            return result.Success(entity);
    //        }

    //        return result.Failed(new EntityError("An unknown error occurred whilst attempting to create an eneity"));

    //    }

    //    public async Task<IActivityResult<Entity>> UpdateAsync(Entity model)
    //    {

    //        var result = new ActivityResult<Entity>();

    //        var user = await _contextFacade.GetAuthenticatedUserAsync();

    //        if (user != null)
    //        {
    //            model.ModifiedUserId = user.Id;
    //        }

    //        model.ModifiedDate = DateTime.UtcNow;

    //        // Validate
    //        if (model.Id <= 0)
    //        {
    //            return result.Failed(new EntityError($"{nameof(model.Id)} must be a valid existing entity id"));
    //        }

    //        if (String.IsNullOrWhiteSpace(model.Title))
    //        {
    //            return result.Failed(new EntityError($"{nameof(model.Title)} is required"));
    //        }

    //        if (String.IsNullOrWhiteSpace(model.Message))
    //        {
    //            return result.Failed(new EntityError($"{nameof(model.Message)} is required"));
    //        }

    //        // Parse Html and message abstract
    //        model.Html = await ParseMarkdown(model.Message);
    //        model.Abstract = await ParseAbstract(model.Message);

    //        // Raise Updating event
    //        Updating?.Invoke(this, new EntityEventArgs(model));

    //        // Publish EntityUpdating event
    //        await _broker.Pub<Entity>(this, new MessageOptions()
    //        {
    //            Key = "EntityUpdating"
    //        }, model);

    //        var entity = await _entityStore.UpdateAsync(model);
    //        if (entity != null)
    //        {

    //            // Raise Updated event
    //            Updated?.Invoke(this, new EntityEventArgs(entity));

    //            // Publish EntityUpdated event
    //            await _broker.Pub<Entity>(this, new MessageOptions()
    //            {
    //                Key = "EntityUpdated"
    //            }, model);

    //            return result.Success(entity);
    //        }

    //        return result.Failed(new EntityError("An unknown error occurred whilst attempting to create an eneity."));

    //    }

    //    public async Task<IActivityResult<Entity>> DeleteAsync(int id)
    //    {

    //        var result = new ActivityResult<Entity>();

    //        var entity = await _entityStore.GetByIdAsync(id);
    //        if (entity == null)
    //        {
    //            return result.Failed(new EntityError($"An entity is the id {id} could not be found"));
    //        }

    //        // Raise Deleting event
    //        Deleting?.Invoke(this, new EntityEventArgs(entity));

    //        // Publish EntityDeleting event
    //        await _broker.Pub<Entity>(this, new MessageOptions()
    //        {
    //            Key = "EntityDeleting"
    //        }, entity);

    //        var success = await _entityStore.DeleteAsync(entity);
    //        if (success)
    //        {

    //            // Raise Deleted event
    //            Deleted?.Invoke(this, new EntityEventArgs(entity, true));

    //            // Publish EntityDeleted event
    //            await _broker.Pub<Entity>(this, new MessageOptions()
    //            {
    //                Key = "EntityDeleted"
    //            }, entity);

    //            return result.Success(entity);
    //        }

    //        return result.Failed(new EntityError("An unknown error occurred whilst attempting to create an eneity."));

    //    }

    //    #endregion

    //    #region "Private Methods"

    //    private async Task<string> ParseMarkdown(string message)
    //    {

    //        foreach (var handler in await _broker.Pub<string>(this, new MessageOptions()
    //        {
    //            Key = "ParseMarkdown"
    //        }, message))
    //        {
    //            return await handler.Invoke(new Message<string>(message, this));
    //        }

    //        return message;

    //    }

    //    private async Task<string> ParseAbstract(string message)
    //    {

    //        foreach (var handler in await _broker.Pub<string>(this, new MessageOptions()
    //        {
    //            Key = "ParseAbstract"
    //        }, message))
    //        {
    //            return await handler.Invoke(new Message<string>(message, this));
    //        }

    //        return message.StripHtml().TrimToAround(500);

    //    }
        
    //    #endregion

    //}

}
