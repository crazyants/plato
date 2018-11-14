using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Abstractions.Models;
using Plato.Mentions;
using Plato.Mentions.Models;
using Plato.Mentions.Services;
using Plato.Mentions.Stores;
using Plato.Notifications.Extensions;

namespace Plato.Discuss.Mentions.Subscribers
{

    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IBroker _broker;
        private readonly IEntityMentionsManager<EntityMention> _entityMentionsManager;
        private readonly IEntityMentionsStore<EntityMention> _entityMentionsStore;
        private readonly IMentionsParser _mentionParser;
        private readonly INotificationManager _notificationManager;
        private readonly ILogger<EntitySubscriber<TEntity>> _logger;

        public EntitySubscriber(
            IEntityMentionsManager<EntityMention> entityMentionsManager,
            IEntityMentionsStore<EntityMention> entityMentionsStore,
            IMentionsParser mentionParser,
            INotificationManager notificationManager,
            IBroker broker,
            ILogger<EntitySubscriber<TEntity>> logger)
        {
       
            _entityMentionsManager = entityMentionsManager;
            _entityMentionsStore = entityMentionsStore;
            _mentionParser = mentionParser;
            _broker = broker;
            _logger = logger;
            _notificationManager = notificationManager;
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
            _broker.Unsub<TEntity>(new MessageOptions()
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

            var userList = users.ToList();

            // Add users mentioned within entity to EntityMentions
            foreach (var user in userList)
            {
                var result = await _entityMentionsManager.CreateAsync(new EntityMention()
                {
                    EntityId = entity.Id,
                    UserId = user.Id
                });
            }

            //await SendNotifications(userList, entity);

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

            // Get users mentioned within message
            var users = await _mentionParser.GetUsersAsync(entity.Message);
            if (users == null)
            {
                return entity;
            }

            // Get all existing mentions
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

            // Build a list of new mentions to add
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

        async Task<TEntity> SendNotifications(IEnumerable<User> users, TEntity entity)
        {

            // Send Notifications
            foreach (var user in users)
            {
                if (user.NotificationEnabled(Notitications.NewMentionWeb))
                {

                    // Build notification
                    var notification = new Notification(Notitications.NewMentionWeb);

                    // Send notification
                    var result = await _notificationManager.SendAsync(notification);

                    // Log any errors
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogCritical(error.Code, error.Description);
                        }
                    }

                }

            }
            
            return entity;

        }

        #endregion

    }

}
