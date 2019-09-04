using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Plato.Entities.Extensions;
using Plato.Entities.Stores;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Issues.Models;
using Plato.Slack.Services;

namespace Plato.Issues.Slack.Subscribers
{

    public class EntityReplySubscriber : IBrokerSubscriber
    {

        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;
        private readonly ILogger<EntitySubscriber> _logger;
        private readonly IEntityStore<Issue> _entityStore;
        private readonly ISlackService _slackService;
        private readonly IBroker _broker;

        public EntityReplySubscriber(
            ICapturedRouterUrlHelper capturedRouterUrlHelper,
            ILogger<EntitySubscriber> logger,
            IEntityStore<Issue> entityStore,
            ISlackService slackService,
            IBroker broker)
        {
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
            _slackService = slackService;
            _entityStore = entityStore;
            _broker = broker;
            _logger = logger;
        }

        // Implementation

        public void Subscribe()
        {

            // Created
            _broker.Sub<Comment>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, async message => await EntityReplyCreated(message.What));

        }

        public void Unsubscribe()
        {

            // Created
            _broker.Unsub<Comment>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, async message => await EntityReplyCreated(message.What));

        }

        // Private methods

        async Task<Comment> EntityReplyCreated(Comment reply)
        {

            // If the created reply is hidden, no need to send notifications
            // Replies can be hidden automatically, for example if they are detected as SPAM
            if (reply.IsHidden())
            {
                return reply; ;
            }

            // We need an entity to build the url
            if (reply.EntityId <= 0)
            {
                return reply;
            }

            // Get entity
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return reply;
            }

            // Build url to entity reply
            var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();
            var url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
            {
                ["area"] = "Plato.Issues",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply.Id
            });

            // Build the message to post to Slack
            var sb = new StringBuilder();
            sb
                .Append("RE: ")
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
                    _logger.LogError($"An error occurred whilst attempting to post a reply to Slack. Response from POST request to Slack Webhook Url: {response.Error}");
                }
            }

            // Continue processing the broker pipeline
            return reply;

        }
        
    }
    
}
