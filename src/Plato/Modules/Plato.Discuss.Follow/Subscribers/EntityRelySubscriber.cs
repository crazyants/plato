using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Discuss.Follow.NotificationTypes;
using Plato.Entities.Models;
using Plato.Follows.Stores;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Extensions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Internal.Tasks.Abstractions;

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
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public EntityReplySubscriber(
            IBroker broker,
            INotificationManager<TEntityReply> notificationManager,
            IFollowStore<Follows.Models.Follow> followStore,
            IDeferredTaskManager deferredTaskManager,
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            IPlatoUserStore<User> platoUserStore)
        {
            _broker = broker;
            _notificationManager = notificationManager;
            _followStore = followStore;
      
            _deferredTaskManager = deferredTaskManager;
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _platoUserStore = platoUserStore;
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

        Task<TEntityReply> EntityReplyCreated(TEntityReply reply)
        {

            if (reply == null)
            {
                throw new ArgumentNullException(nameof(reply));
            }

            // No need to send notifications for hidden replies
            if (reply.IsHidden)
            {
                return Task.FromResult(reply);
            }

            // No need to send notifications for replies flagged as spam
            if (reply.IsSpam)
            {
                return Task.FromResult(reply);
            }
            
            // Defer notifications to first available thread pool thread
            _deferredTaskManager.AddTask(async context =>
            {
                
                // Get all follows for topic
                var follows = await _followStore.QueryAsync()
                    .Select<FollowQueryParams>(q =>
                    {
                        q.ThingId.Equals(reply.EntityId);
                        q.Name.Equals(FollowTypes.Topic.Name);
                    })
                    .ToList();
                
                // No follows simply return
                if (follows?.Data == null)
                {
                    return;
                }

                // Get a collection of all users to notify
                // Exclude the author so they are not notified of there own posts
                var users = await _platoUserStore.QueryAsync()
                    .Select<UserQueryParams>(q =>
                    {
                        q.Id.IsIn(follows.Data
                            .Where(f => f.CreatedUserId != reply.CreatedUserId)
                            .Select(f => f.CreatedUserId)
                            .ToArray());
                    })
                    .ToList();

                // No follows simply return
                if (users?.Data == null)
                {
                    return;
                }

                // Send mention notifications
                foreach (var user in users.Data)
                {

                    // Email notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewReply))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.NewReply)
                        {
                            To = user,
                        }, reply);
                    }

                    // Web notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewReply))
                    {
                        await _notificationManager.SendAsync(new Notification(WebNotifications.NewReply)
                        {
                            To = user,
                            From = new User()
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
                
            });
            
            return Task.FromResult(reply);

        }

        Task<TEntityReply> EntityReplyUpdated(TEntityReply reply)
        {
            // No need to send notifications for reply updates
            // May be implemented at a later stage
            return Task.FromResult(reply);
        }
        
        #endregion

    }

}
