using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Questions.Mentions.NotificationTypes;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Extensions;
using Plato.Mentions.Models;
using Plato.Mentions.Services;
using Plato.Mentions.Stores;
using Plato.Entities.Extensions;

namespace Plato.Questions.Mentions.Subscribers
{
    
    public class EntityReplySubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {

        private readonly IEntityMentionsManager<EntityMention> _entityMentionsManager;
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly IEntityMentionsStore<EntityMention> _entityMentionsStore;        
        private readonly INotificationManager<TEntityReply> _notificationManager;
        private readonly ILogger<EntityReplySubscriber<TEntityReply>> _logger;
        private readonly IMentionsParser _mentionParser;
        private readonly IBroker _broker;

        public EntityReplySubscriber(
            IEntityMentionsManager<EntityMention> entityMentionsManager,
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            IEntityMentionsStore<EntityMention> entityMentionsStore,
            INotificationManager<TEntityReply> notificationManager,
            ILogger<EntityReplySubscriber<TEntityReply>> logger,
            IMentionsParser mentionParser,
            IBroker broker)
        {
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _entityMentionsManager = entityMentionsManager;
            _entityMentionsStore = entityMentionsStore;
            _notificationManager = notificationManager;
            _mentionParser = mentionParser;
            _broker = broker;
            _logger = logger;
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
            }, async message => await EntityReplyUpdated(message.What));

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
            }, async message => await EntityReplyUpdated(message.What));

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
            
            // No need to send @mention notifications if the reply is hidden
            if (reply.IsHidden())
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
            var usersToNotify = new List<User>();
            foreach (var user in users)
            {
                var result = await _entityMentionsManager.CreateAsync(new EntityMention()
                {
                    EntityId = reply.EntityId,
                    EntityReplyId = reply.Id,
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
            await SendNotifications(usersToNotify, reply);

            return reply;

        }

        async Task<TEntityReply> EntityReplyUpdated(TEntityReply reply)
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

            // No need to send @mention notifications if the reply is hidden
            if (reply.IsHidden())
            {
                return reply;
            }

            // Get users mentioned within message
            var users = await _mentionParser.GetUsersAsync(reply.Message);
            if (users == null)
            {
                return reply;
            }

            // Get all existing mentions
            var mentions = await _entityMentionsStore.QueryAsync()
                .Select<EntityMentionsQueryParams>(q =>
                {
                    q.EntityReplyId.Equals(reply.Id);
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
                        EntityReplyId = reply.Id,
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

            return reply;

        }

        async Task<TEntityReply> SendNotifications(IEnumerable<User> users, TEntityReply reply)
        {
            // Send mention notifications
            foreach (var user in users)
            {

                // Email notifications
                if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewMention))
                {
                    await _notificationManager.SendAsync(new Notification(EmailNotifications.NewMention)
                    {
                        To = user,
                    }, reply);
                }

                // Web notifications
                if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewMention))
                {
                    await _notificationManager.SendAsync(new Notification(WebNotifications.NewMention)
                    {
                        To = user,
                    }, reply);
                }

            }

            return reply;

        }

        #endregion

    }

}
