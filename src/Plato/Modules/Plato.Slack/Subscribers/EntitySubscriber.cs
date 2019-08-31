using System.Text;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Extensions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Slack.Services;

namespace Plato.Slack.Subscribers
{

    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {
        
        private readonly ISlackService _slackService;
        private readonly IBroker _broker;

        public EntitySubscriber(
            ISlackService slackService,
            IBroker broker)
        {
           
            _slackService = slackService;
            _broker = broker;
        }

        // Implementation

        public void Subscribe()
        {
            // Subscribe to the EntityCreated event
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

        }

        public void Unsubscribe()
        {
            // Unsubscribe from the EntityCreated event
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

        }

        // Private Methods

        async Task<TEntity> EntityCreated(TEntity entity)
        {
            
            // If the created entity is hidden, no need to send notifications
            // Entities can be hidden automatically, for example if they are detected as SPAM
            if (entity.IsHidden())
            {
                return entity; ;
            }
            
            // Build our message to post to our Slack channel
            var sb = new StringBuilder();
            sb.Append(entity.Title);

            // Finally post our message to Slack
            var response = await _slackService.PostAsync(sb.ToString());

            if (response.Success)
            {

            }

            // Continue processing the broker pipeline
            return entity;

        }

    }

}
