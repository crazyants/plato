using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Discuss.Follow.NotificationTypes;
using Plato.Discuss.ViewComponents;
using Plato.Entities.Models;
using Plato.Follows.Stores;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Notifications.Extensions;

namespace Plato.Discuss.Follow.Subscribers
{
  
    /// <summary>
    /// Triggers all entity follow notifications when replies are posted.
    /// </summary>
    /// <typeparam name="TEntityReply"></typeparam>
    public class EntityReplySubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {
        
        private readonly IBroker _broker;
        private readonly INotificationManager<TEntityReply> _notificationManager;
        private readonly IFollowStore<Follows.Models.Follow> _followStore;

        public EntityReplySubscriber(
            IBroker broker,
            INotificationManager<TEntityReply> notificationManager,
            IFollowStore<Follows.Models.Follow> followStore)
        {
            _broker = broker;
            _notificationManager = notificationManager;
            _followStore = followStore;
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

            // No need to send notifications for private replies
            if (reply.IsPrivate)
            {
                return reply;
            }

            // No need to send notifications for replies flagged as spam
            if (reply.IsSpam)
            {
                return reply;
            }
            
            // Get all follows for topic
            var follows = await _followStore.QueryAsync()
                .Select<FollowQueryParams>(q =>
                {
                    q.ThingId.Equals(reply.EntityId);
                    q.Name.Equals(FollowTypes.Topic.Name);
                })
                .ToList();
            
            // No follows simply return
            if (follows == null)
            {
                return reply;
            }

            // No follows simply return
            if (follows.Data == null)
            {
                return reply;
            }

            // Build a collection of all users to notify
            // Exclude the reply author so they are not
            // notified of there own replies
            var users = new List<User>();
            foreach (var follow in follows.Data)
            {
                var isMotAuthor = follow.CreatedUserId != reply.CreatedUserId;
                if (isMotAuthor)
                {
                    users.Add((User)follow.CreatedBy);
                }
            }
            
            // Send notifications
            await SendNotifications(users, reply);
            
            return reply;

        }

        Task<TEntityReply> EntityReplyUpdated(TEntityReply reply)
        {
            // No need to send notifications for reply updates
            // May be implemented at a later stage
            return Task.FromResult(reply);
        }

        async Task<TEntityReply> SendNotifications(IEnumerable<User> users, TEntityReply reply)
        {
            // Send mention notifications
            foreach (var user in users)
            {

                // Email notifications
                if (user.NotificationEnabled(EmailNotifications.NewReply))
                {
                    await _notificationManager.SendAsync(new Notification(EmailNotifications.NewReply)
                    {
                        To = user,
                    }, reply);
                }
                
                // Web notifications
                if (user.NotificationEnabled(WebNotifications.NewReply))
                {
                    await _notificationManager.SendAsync(new Notification(WebNotifications.NewReply)
                    {
                        To = user,
                        From = new User
                        {
                            Id = reply.CreatedBy.Id,
                            UserName = reply.CreatedBy.UserName,
                            DisplayName = reply.CreatedBy.DisplayName,
                            Alias = reply.CreatedBy.Alias,
                            PhotoUrl = reply.CreatedBy.PhotoUrl,
                            PhotoColor = reply.CreatedBy.PhotoColor
                        }
                    }, reply);
                }

            }

            return reply;

        }
        
        #endregion

    }

}
