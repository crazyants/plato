using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Extensions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Slack.Subscribers
{

    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IBroker _broker;

        public EntitySubscriber(IBroker broker)
        {
            _broker = broker;
        }

        // Implementation

        public void Subscribe()
        {
            // Created
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

        }

        public void Unsubscribe()
        {
            // Created
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

        }

        // Private Methods

        Task<TEntity> EntityCreated(TEntity entity)
        {
            
            // If the entity is hidden, no need to send notifications
            if (entity.IsHidden())
            {
                return Task.FromResult(entity); ;
            }

            return Task.FromResult(entity);

        }

    }

}
