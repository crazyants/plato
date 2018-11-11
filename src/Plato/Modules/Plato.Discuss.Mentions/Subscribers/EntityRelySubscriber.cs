using System;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Messaging.Abstractions;
using Plato.Mentions.Models;
using Plato.Mentions.Services;

namespace Plato.Discuss.Mentions.Subscribers
{
    
    /// <typeparam name="TEntityReply"></typeparam>
    public class EntityReplySubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {

        private readonly IBroker _broker;
        private readonly IEntityMentionsManager<EntityMention> _entityMentionsManager;
        private readonly IMentionsParser _mentionParser;

        public EntityReplySubscriber(
            IEntityMentionsManager<EntityMention> entityMentionsManager,
            IMentionsParser mentionParser,
            IBroker broker)
        {
            _mentionParser = mentionParser;
            _entityMentionsManager = entityMentionsManager;
            _broker = broker;
        }
        
        #region "Implementation"

        public void Subscribe()
        {
            // Created
            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, async message => await EntityReplyCreated(message.What));

            // Updated
            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdated"
            }, async message => await EntityReplyCreated(message.What));

        }

        public void Unsubscribe()
        {
            // Created
            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, async message => await EntityReplyCreated(message.What));

            // Updated
            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdated"
            }, async message => await EntityReplyCreated(message.What));

        }
        
        public void Dispose()
        {
            Unsubscribe();
        }

        #endregion

        #region "Private Methods"

        async Task<TEntityReply> EntityReplyCreated(TEntityReply reply)
        {

            if (reply == null)
            {
                throw new ArgumentNullException(nameof(reply));
            }

            // If  we don't have a message we can't parse mentions
            if (String.IsNullOrEmpty(reply.Message))
            {
                return reply;
            }

            // Get users mentioned within entity message
            var users = await _mentionParser.GetUsersAsync(reply.Message);
            if (users == null)
            {
                return reply;
            }

            // Add users mentioned within entity to EntityMentions
            foreach (var user in users)
            {
                var result = await _entityMentionsManager.CreateAsync(new EntityMention()
                {
                    EntityId = reply.EntityId,
                    EntityReplyId = reply.Id,
                    UserId = user.Id
                });
            }

            return reply;

        }
        
        #endregion

    }

}
