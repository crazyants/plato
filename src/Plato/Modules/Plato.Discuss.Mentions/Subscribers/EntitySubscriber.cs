using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Mentions.Models;
using Plato.Mentions.Services;
using Plato.Mentions.Stores;

namespace Plato.Discuss.Mentions.Subscribers
{

    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IBroker _broker;
        private readonly IEntityMentionsManager<EntityMention> _entityMentionsManager;
        private readonly IEntityMentionsStore<EntityMention> _entityMentionsStore;
        private readonly IMentionsParser _mentionParser;

        public EntitySubscriber(
            IEntityMentionsManager<EntityMention> entityMentionsManager,
            IEntityMentionsStore<EntityMention> entityMentionsStore,
            IMentionsParser mentionParser,
            IBroker broker)
        {
       
            _entityMentionsManager = entityMentionsManager;
            _entityMentionsStore = entityMentionsStore;
            _mentionParser = mentionParser;
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
            // Created
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

            // Updated
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated"
            }, async message => await EntityUpdated(message.What));

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

        async Task<TEntity> EntityUpdated(TEntity entity)
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

            // Get existing mentions
            var mentions = await _entityMentionsStore.QueryAsync()
                .Select<EntityMentionsQueryParams>(q =>
                {
                    q.EntityId.Equals(entity.Id);
                })
                .OrderBy("Id", OrderBy.Asc)
                .ToList();
            
            var mentionedUsers = users.ToList();
            var existingMentions = mentions?.Data.ToList();
            var mentionsToAdd = new List<EntityMention>();
            var mentionsToRemove = new List<EntityMention>();

            // Build a list of new mentions
            foreach (var user in mentionedUsers)
            {
                // Is there an existing mention for the user?
                var existingMention = existingMentions?.FirstOrDefault(m => m.UserId == user.Id);
                if (existingMention == null)
                {
                    mentionsToAdd.Add(new EntityMention()
                    {
                        EntityId = entity.Id,
                        UserId = user.Id
                    });
                }
            }

            // Build list of mentions to remove
            if (existingMentions != null)
            {
                foreach (var mention in existingMentions)
                {
                    // Is user still mentioned within message?
                    var mentionedUser = mentionedUsers.FirstOrDefault(m => m.Id == mention.UserId);
                    if (mentionedUser == null)
                    {
                        mentionsToRemove.Add(mention);
                    }
                }
            }

            // Delete removed mentions
            foreach (var mention in mentionsToRemove)
            {
                await _entityMentionsManager.DeleteAsync(mention);
            }

            // Add new users mentioned within entity to EntityMentions
            foreach (var mention in mentionsToAdd)
            {
               await _entityMentionsManager.CreateAsync(mention);
            }

            return entity;

        }


        #endregion

    }

}
