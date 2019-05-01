using System.Linq;
using System.Threading.Tasks;
using Plato.Categories.Repositories;
using Plato.Categories.Models;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Categories.Subscribers
{

    /// <summary>
    /// Updates a categories default sort order via CategoryCreating and CategoryUpdating.
    /// </summary>
    /// <typeparam name="TCategory"></typeparam>
    public class CategorySubscriber<TCategory> : IBrokerSubscriber where TCategory : class, ICategory
    {

        private readonly ICategoryRepository<TCategory> _entityRepository;
     
        private readonly IBroker _broker;

        public CategorySubscriber(
            ICategoryRepository<TCategory> entityRepository,
            IBroker broker)
        {
            _entityRepository = entityRepository;
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {

            // Creating
            _broker.Sub<TCategory>(new MessageOptions()
            {
                Key = "CategoryCreating"
            }, async message => await CategoryCreating(message.What));
            
            // Updating
            _broker.Sub<TCategory>(new MessageOptions()
            {
                Key = "CategoryUpdating"
            }, async message => await CategoryUpdating(message.What));

        }

        public void Unsubscribe()
        {

            // Creating
            _broker.Unsub<TCategory>(new MessageOptions()
            {
                Key = "CategoryCreating"
            }, async message => await CategoryCreating(message.What));
            
            // Updating
            _broker.Unsub<TCategory>(new MessageOptions()
            {
                Key = "CategoryUpdating"
            }, async message => await CategoryUpdating(message.What));

        }

        #endregion

        #region "Private Methods"
        
        async Task<TCategory> CategoryCreating(TCategory entity)
        {
            // Get the next available sort order for new entries
            if (entity.SortOrder == 0)
            {
                entity.SortOrder = await GetNextAvailableSortOrder(entity);
            }

            return entity;

        }
        
        async Task<TCategory> CategoryUpdating(TCategory entity)
        {

            // Get existing entity before any changes
            var existingEntity = await _entityRepository.SelectByIdAsync(entity.Id);

            // We need an existing entity
            if (existingEntity == null)
            {
                return entity;
            }
            
            // If the parent changes ensure we update the sort order
            if (entity.ParentId != existingEntity.ParentId)
            {
                entity.SortOrder = await GetNextAvailableSortOrder(entity);
            }
         
            // Return
            return entity;

        }
        
        async Task<int> GetNextAvailableSortOrder(TCategory model)
        {

            var sortOrder = 0;
            var entities = await _entityRepository
                .SelectByFeatureIdAsync(model.FeatureId);

            if (entities != null)
            {
                foreach (var entity in entities
                    .Where(c => c.ParentId == model.ParentId)
                    .OrderBy(o => o.SortOrder))
                {
                    sortOrder = entity.SortOrder;
                }
            }

            return sortOrder + 1;

        }
        
        #endregion

    }

}
