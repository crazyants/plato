using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Discuss.Channels.Follow.NotificationTypes;
using Plato.Entities.Models;
using Plato.Follows.Stores;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Extensions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Discuss.Channels.Follow.Subscribers
{
    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IBroker _broker;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly INotificationManager<TEntity> _notificationManager;
        private readonly IFollowStore<Follows.Models.Follow> _followStore;
        private readonly IUserDataDecorator _userDataDecorator;
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;

        public EntitySubscriber(
            IBroker broker,
            IDeferredTaskManager deferredTaskManager,
            INotificationManager<TEntity> notificationManager,
            IFollowStore<Follows.Models.Follow> followStore,
            IUserDataDecorator userDataDecorator,
            IUserNotificationTypeDefaults userNotificationTypeDefaults)
        {
            _broker = broker;
            _deferredTaskManager = deferredTaskManager;
            _notificationManager = notificationManager;
            _followStore = followStore;
            _userDataDecorator = userDataDecorator;
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
        }

        #region "Implementation"

        public void Subscribe()
        {
            // Created
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated",
                Order = short.MaxValue
            }, async message => await EntityCreated(message.What));

            // Updated
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated",
                Order = short.MaxValue
            }, async message => await EntityUpdated(message.What));

        }

        public void Unsubscribe()
        {
            // Created
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated",
                Order = short.MaxValue
            }, async message => await EntityCreated(message.What));

            // Updated
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated",
                Order = short.MaxValue
            }, async message => await EntityUpdated(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<TEntity> EntityCreated(TEntity entity)
        {
            entity = await SendNotificationsForChannel(entity);
            return await SendNotificationsForAllChannels(entity);
        }

        Task<TEntity> EntityUpdated(TEntity entity)
        {
            // Category notifications are not triggered for entity updates
            // This could possibly be implemented at a later stage
            return Task.FromResult(entity);
        }

        Task<TEntity> SendNotificationsForChannel(TEntity entity)
        {


            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // The entity is NOT posted within a specific category so no need to send notifications
            if (entity.CategoryId == 0)
            {
                return Task.FromResult(entity);
            }

            // No need to send notifications for entities flagged as private
            if (entity.IsPrivate)
            {
                return Task.FromResult(entity);
            }

            // No need to send notifications for entities flagged as spam
            if (entity.IsSpam)
            {
                return Task.FromResult(entity);
            }

            // Defer notifications to first available thread pool thread
            _deferredTaskManager.AddTask(async context =>
            {

                // Get all follows for channel
                var follows = await _followStore.QueryAsync()
                    .Select<FollowQueryParams>(q =>
                    {
                        q.ThingId.Equals(entity.CategoryId);
                        q.Name.Equals(FollowTypes.Channel.Name);
                    })
                    .ToList();

                // No follows simply return
                if (follows?.Data == null)
                {
                    return;
                }

                // Build a collection of all users to notify
                // Exclude the author so they are not notified of there own posts
                var users = new List<User>(follows.Data.Count);
                foreach (var follow in follows.Data)
                {
                    var isMotAuthor = follow.CreatedUserId != entity.CreatedUserId;
                    if (isMotAuthor)
                    {
                        users.Add((User)follow.CreatedBy);
                    }
                }

                // Merge user data so we know the opt-in status for notifications
                // This is critical otherwise NotificationEnabled will always return false
                var mergedUsers = await _userDataDecorator.DecorateAsync(users);

                // Send mention notifications
                foreach (var user in mergedUsers)
                {

                    // Email notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewTopic))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.NewTopic)
                        {
                            To = user,
                        }, entity);
                    }

                    // Web notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewTopic))
                    {
                        await _notificationManager.SendAsync(new Notification(WebNotifications.NewTopic)
                        {
                            To = user,
                            From = new User
                            {
                                Id = entity.CreatedBy.Id,
                                UserName = entity.CreatedBy.UserName,
                                DisplayName = entity.CreatedBy.DisplayName,
                                Alias = entity.CreatedBy.Alias,
                                PhotoUrl = entity.CreatedBy.PhotoUrl,
                                PhotoColor = entity.CreatedBy.PhotoColor
                            }
                        }, entity);
                    }

                }

            });

            return Task.FromResult(entity);

        }

        Task<TEntity> SendNotificationsForAllChannels(TEntity entity)
        {
            

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            // No need to send notifications for entities flagged as private
            if (entity.IsPrivate)
            {
                return Task.FromResult(entity);
            }

            // No need to send notifications for entities flagged as spam
            if (entity.IsSpam)
            {
                return Task.FromResult(entity);
            }

            // Defer notifications to first available thread pool thread
            _deferredTaskManager.AddTask(async context =>
            {

                // Get all follows for channel
                var follows = await _followStore.QueryAsync()
                    .Select<FollowQueryParams>(q =>
                    {
                        q.Name.Equals(FollowTypes.AllChannels.Name);
                    })
                    .ToList();

                // No follows simply return
                if (follows?.Data == null)
                {
                    return;
                }

                // Build a collection of all users to notify
                // Exclude the author so they are not notified of there own posts
                var users = new List<User>(follows.Data.Count);
                foreach (var follow in follows.Data)
                {
                    var isMotAuthor = follow.CreatedUserId != entity.CreatedUserId;
                    if (isMotAuthor)
                    {
                        users.Add((User)follow.CreatedBy);
                    }
                }

                // Merge user data so we know the opt-in status for notifications
                // This is critical otherwise NotificationEnabled will always return false
                var mergedUsers = await _userDataDecorator.DecorateAsync(users);

                // Send mention notifications
                foreach (var user in mergedUsers)
                {

                    // Email notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewTopic))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.NewTopic)
                        {
                            To = user,
                        }, entity);
                    }

                    // Web notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewTopic))
                    {
                        await _notificationManager.SendAsync(new Notification(WebNotifications.NewTopic)
                        {
                            To = user,
                            From = new User
                            {
                                Id = entity.CreatedBy.Id,
                                UserName = entity.CreatedBy.UserName,
                                DisplayName = entity.CreatedBy.DisplayName,
                                Alias = entity.CreatedBy.Alias,
                                PhotoUrl = entity.CreatedBy.PhotoUrl,
                                PhotoColor = entity.CreatedBy.PhotoColor
                            }
                        }, entity);
                    }

                }

            });

            return Task.FromResult(entity);

        }
        
        #endregion

    }

}
