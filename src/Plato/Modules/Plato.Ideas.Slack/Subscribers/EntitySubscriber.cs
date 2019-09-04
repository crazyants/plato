using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Plato.Entities.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Ideas.Models;
using Plato.Slack.Services;

namespace Plato.Ideas.Slack.Subscribers
{

    public class EntitySubscriber : IBrokerSubscriber
    {

        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;
        private readonly ILogger<EntitySubscriber> _logger;
        private readonly ISlackService _slackService;
        private readonly IBroker _broker;

        public EntitySubscriber(
            ICapturedRouterUrlHelper capturedRouterUrlHelper,
            ILogger<EntitySubscriber> logger,
            ISlackService slackService,
            IBroker broker)
        {
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
            _slackService = slackService;
            _broker = broker;
            _logger = logger;
        }

        // Implementation

        public void Subscribe()
        {
            // Subscribe to the EntityCreated event
            _broker.Sub<Idea>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

        }

        public void Unsubscribe()
        {
            // Unsubscribe from the EntityCreated event
            _broker.Unsub<Idea>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

        }

        // Private Methods

        async Task<Idea> EntityCreated(Idea entity)
        {
            
            // If the created entity is hidden, no need to send notifications
            // Entities can be hidden automatically, for example if they are detected as SPAM
            if (entity.IsHidden())
            {
                return entity; ;
            }

            // Build url to the entity
            var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();
            var url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
            {
                ["area"] = "Plato.Ideas",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            });

            // Build the message to post to Slack
            var sb = new StringBuilder();
            sb
                .Append(entity.Title)
                .Append(" - ")
                .Append(baseUri)
                .Append(url);

            // Finally post our message to Slack
            var response = await _slackService.PostAsync(sb.ToString());

            // Log any errors that may have occurred
            if (!response.Success)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"An error occurred whilst attempting to post to Slack. Response from POST request to Slack Webhook Url: {response.Error}");
                }
            }

            // Continue processing the broker pipeline
            return entity;

        }

    }

}
