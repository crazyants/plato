using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Mentions.Models;
using Plato.Discuss.Mentions.NotificationTypes;
using Plato.Mentions.Services;
using Plato.Mentions.Stores;
using Plato.Notifications.Extensions;

namespace Plato.Discuss.Mentions.Subscribers
{

    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {
        
        private readonly IEntityMentionsManager<EntityMention> _entityMentionsManager;
        private readonly IEntityMentionsStore<EntityMention> _entityMentionsStore;
        private readonly INotificationManager<TEntity> _notificationManager;
        private readonly IMentionsParser _mentionParser;
        private readonly ILogger<EntitySubscriber<TEntity>> _logger;
        private readonly IBroker _broker;

        public EntitySubscriber(
            IEntityMentionsManager<EntityMention> entityMentionsManager,
            IEntityMentionsStore<EntityMention> entityMentionsStore,
            INotificationManager<TEntity> notificationManager,
            IMentionsParser mentionParser,
            ILogger<EntitySubscriber<TEntity>> logger,
            IBroker broker)
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

            // Add users mentioned within entity to EntityMentions
            var usersToNotify = new List<User>();
            foreach (var user in users)
            {
                var result = await _entityMentionsManager.CreateAsync(new EntityMention()
                {
                    EntityId = entity.Id,
                    UserId = user.Id
                });
                if (result.Succeeded)
                {
                    usersToNotify.Add(user);
                }
                else
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogCritical(error.Code, error.Description);
                        }
                    }
                }
            }

            // Send mention notifications
            await SendNotifications(usersToNotify, entity);

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
            // Send mention notifications
            foreach (var user in users)
            {

                // Email notifications
                if (user.NotificationEnabled(EmailNotifications.NewMention))
                {
                    await _notificationManager.SendAsync(new Notification(EmailNotifications.NewMention)
                    {
                        To = user,
                    }, entity);
                }

                // Web notifications
                if (user.NotificationEnabled(WebNotifications.NewMention))
                {
                    await _notificationManager.SendAsync(new Notification(WebNotifications.NewMention)
                    {
                        To = user,
                    }, entity);
                }

            }
            
            return entity;

        }

        #endregion

    }

}
