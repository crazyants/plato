using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Entities.History.Subscribers
{

    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IBroker _broker;
      
        public EntitySubscriber(
            IBroker broker)
        {
            _broker = broker;
  
        }

        #region "Implementation"

        public void Subscribe()
        {
            // Created
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

            // Updated
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

        Task<TEntity> EntityCreated(TEntity entity)
        {

            return Task.FromResult(entity);
            //// No need to update cateogry for private entities
            //if (entity.IsPrivate)
            //{
            //    return entity;
            //}

            //// No need to update cateogry for soft deleted entities
            //if (entity.IsDeleted)
            //{
            //    return entity;
            //}

            //// No need to update cateogry for entities flagged as spam
            //if (entity.IsSpam)
            //{
            //    return entity;
            //}

            //// Ensure we have a categoryId for the entity
            //if (entity.CategoryId <= 0)
            //{
            //    return entity;
            //}

            //// Ensure we found the category
            //var channel = await _channelStore.GetByIdAsync(entity.CategoryId);
            //if (channel == null)
            //{
            //    return entity;
            //}

            //// Update channel details
            //await _channelDetailsUpdater.UpdateAsync(channel.Id);

            //// Return
            //return entity;

        }

        Task<TEntity> EntityUpdated(TEntity entity)
        {

            return Task.FromResult(entity);
            //// No need to update cateogry for private entities
            //if (entity.IsPrivate)
            //{
            //    return entity;
            //}

            //// No need to update cateogry for soft deleted entities
            //if (entity.IsDeleted)
            //{
            //    return entity;
            //}

            //// No need to update cateogry for entities flagged as spam
            //if (entity.IsSpam)
            //{
            //    return entity;
            //}

            //// Ensure we have a categoryId for the entity
            //if (entity.CategoryId <= 0)
            //{
            //    return entity;
            //}

            //// Ensure we found the category
            //var channel = await _channelStore.GetByIdAsync(entity.CategoryId);
            //if (channel == null)
            //{
            //    return entity;
            //}

            //// Update channel details
            //await _channelDetailsUpdater.UpdateAsync(channel.Id);

            //// Return
            //return entity;

        }

        #endregion

    }

}
