using Plato.Entities.Models;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Discuss.Services
{
    public class EntityReplySubscriber : IBrokerSubscriber
    {

        private readonly IBroker _broker;

        public EntityReplySubscriber(IBroker broker)
        {
            _broker = broker;
        }

        public void Subscribe()
        {
            // Subscribe to EntityReplyCreated event
            _broker.Sub<EntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, (entityReply) =>
            {


            });

            // Subscribe to EntityReplyUpdated event
            _broker.Sub<EntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdated"
            }, (entityReply) =>
            {


            });
            
            // Subscribe to EntityReplyDeeted event
            _broker.Sub<EntityReply>(new MessageOptions()
            {
                Key = "EntityReplyDeeted"
            }, (entityReply) =>
            {


            });

        }

        public void Unsubscribe()
        {
            throw new System.NotImplementedException();
        }


        public void Dispose()
        {
       
        }

    }

}
