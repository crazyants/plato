using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Messaging.Abstractions;
using Plato.Mentions.Models;
using Plato.Mentions.Services;

namespace Plato.Mentions.Subscribers
{

    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IBroker _broker;
        private readonly IEntityMentionsManager<EntityMention> _entityMentionsManager;
        private readonly IMentionsParser _mentionParser;

        public EntitySubscriber(
            IBroker broker,
            IMentionsParser mentionParser,
            IEntityMentionsManager<EntityMention> entityMentionsManager)
        {
            _broker = broker;
        
            _mentionParser = mentionParser;
            _entityMentionsManager = entityMentionsManager;
        }

        #region "Implementation"

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
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));
        }

        #endregion

        #region "Private Methods"

        async Task<TEntity> EntityCreated(TEntity entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // If  we don't have a message we can't parse mentions
            if (String.IsNullOrEmpty(entity.Message))
            {
                return entity;
            }

            // Get users mentioned within entity message
            var users = await _mentionParser.GetUsersAsync(entity.Message);
            if (users == null)
            {
                return entity;
            }

            // Add users mentioned within entity to EntityMentions
            foreach (var user in users)
            {
                var result = await _entityMentionsManager.CreateAsync(new EntityMention()
                {
                    EntityId = entity.Id,
                    UserId = user.Id
                });
            }

            return entity;

        }

        #endregion

    }

}
