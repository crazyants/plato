using System.Threading.Tasks;
using Plato.Categories.Stores;
using Plato.Docs.Categories.Models;
using Plato.Docs.Categories.Services;
using Plato.Entities.Models;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Docs.Categories.Subscribers
{

    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IBroker _broker;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly ICategoryDetailsUpdater _categoryDetailsUpdater;

        public EntitySubscriber(
            IBroker broker,
            ICategoryStore<Category> categoryStore,
            ICategoryDetailsUpdater categoryDetailsUpdater)
        {
            _broker = broker;
            _categoryStore = categoryStore;
            _categoryDetailsUpdater = categoryDetailsUpdater;
        }

        #region "Implementation"

        public void Subscribe()
        {
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated"
            }, async message => await EntityUpdated(message.What));

        }

        public void Unsubscribe()
        {
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated"
            }, async message => await EntityUpdated(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<TEntity> EntityCreated(TEntity entity)
        {
            
            // Ensure we have a category for the entity
            if (entity.CategoryId <= 0)
            {
                return entity;
            }

            // Ensure we found the category
            var category = await _categoryStore.GetByIdAsync(entity.CategoryId);
            if (category == null)
            {
                return entity;
            }

            // Update category details
            await _categoryDetailsUpdater.UpdateAsync(category.Id);

            // return 
            return entity;

        }

        async Task<TEntity> EntityUpdated(TEntity entity)
        {
            
            // Ensure we have a category for the entity
            if (entity.CategoryId <= 0)
            {
                return entity;
            }

            // Ensure we found the category
            var channel = await _categoryStore.GetByIdAsync(entity.CategoryId);
            if (channel == null)
            {
                return entity;
            }

            // Update category details
            await _categoryDetailsUpdater.UpdateAsync(channel.Id);

            // return 
            return entity;

        }

        #endregion

    }

}
