using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Discuss.Channels.Subscribers
{

    //public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    //{

    //    private readonly IBroker _broker;

    //    public EntitySubscriber(IBroker broker)
    //    {
    //        _broker = broker;
    //    }

    //    #region "Implementation"

    //    public void Subscribe()
    //    {
    //        // Created
    //        _broker.Sub<TEntity>(new MessageOptions()
    //        {
    //            Key = "EntityCreated"
    //        }, async message => await EntityCreated(message.What));

    //        // Updated
    //        _broker.Sub<TEntity>(new MessageOptions()
    //        {
    //            Key = "EntityUpdated"
    //        }, async message => await EntityUpdated(message.What));

    //        // Deleted
    //        _broker.Sub<TEntity>(new MessageOptions()
    //        {
    //            Key = "EntityDeleted"
    //        }, async message => await EntityDeleted(message.What));
    //    }

    //    public void Unsubscribe()
    //    {

    //        // Created
    //        _broker.Unsub<TEntity>(new MessageOptions()
    //        {
    //            Key = "EntityCreated"
    //        }, async message => await EntityCreated(message.What));

    //        // Updated
    //        _broker.Unsub<TEntity>(new MessageOptions()
    //        {
    //            Key = "EntityUpdated"
    //        }, async message => await EntityUpdated(message.What));

    //        // Deleted
    //        _broker.Unsub<TEntity>(new MessageOptions()
    //        {
    //            Key = "EntityDeleted"
    //        }, async message => await EntityDeleted(message.What));

    //    }

    //    #endregion

    //    #region "Private Methods"

    //    Task<TEntity> EntityCreated(TEntity entity)
    //    {
    //        return Task.FromResult(entity);
    //    }

    //    Task<TEntity> EntityUpdated(TEntity entity)
    //    {
    //        return Task.FromResult(entity);
    //    }

    //    Task<TEntity> EntityDeleted(TEntity entity)
    //    {
    //        return Task.FromResult(entity);
    //    }

    //    #endregion

    //}

}
