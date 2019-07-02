using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Discuss.Categories.Follow.NotificationTypes;
using Plato.Discuss.Models;
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
using Plato.Entities.Extensions;
using Plato.Entities.Stores;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Discuss.Categories.Follow.Subscribers
{
    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {
        
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly INotificationManager<TEntity> _notificationManager;
        private readonly IFollowStore<Follows.Models.Follow> _followStore;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IEntityStore<TEntity> _entityStore;
        private readonly IBroker _broker;

        public EntitySubscriber(
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            INotificationManager<TEntity> notificationManager,
            IFollowStore<Follows.Models.Follow> followStore,
            IDeferredTaskManager deferredTaskManager,
            IPlatoUserStore<User> platoUserStore,
            IEntityStore<TEntity> entityStore,
            IBroker broker)
        {
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _deferredTaskManager = deferredTaskManager;
            _notificationManager = notificationManager;
            _platoUserStore = platoUserStore;
            _followStore = followStore;
            _entityStore = entityStore;
            _broker = broker;
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
            entity = await SendNotificationsForCategory(entity);
            return await SendNotificationsForAllCategories(entity);
        }

        async Task<TEntity> EntityUpdated(TEntity entity)
        {
            entity = await SendNotificationsForCategory(entity);
            return await SendNotificationsForAllCategories(entity);
        }

        Task<TEntity> SendNotificationsForCategory(TEntity entity)
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

            // No need to send notifications for hidden entities
            if (entity.IsHidden())
            {
                return Task.FromResult(entity);
            }

            // Defer notifications to first available thread pool thread
            _deferredTaskManager.AddTask(async context =>
            {
               
               // Get all follows for category the entity was posted to
                var follows = await _followStore.QueryAsync()
                    .Select<FollowQueryParams>(q =>
                    {
                        q.ThingId.Equals(entity.CategoryId);
                        q.Name.Equals(FollowTypes.Category.Name);
                    })
                    .ToList();
                
                // No follows simply return
                if (follows?.Data == null)
                {
                    return;
                }

                // Get follow state for entity
                var state = entity.GetOrCreate<FollowState>();

                // Have category notifications already been sent for the entity?
                var follow = state.FollowsSent.FirstOrDefault(f =>
                    f.Name.Equals(FollowTypes.Category.Name, StringComparison.InvariantCultureIgnoreCase));
                if (follow != null && follow.Sent)
                {
                    return;
                }

                // Get a collection of all users to notify
                // Exclude the author so they are not notified of there own posts
                var users = await _platoUserStore.QueryAsync()
                    .Select<UserQueryParams>(q =>
                    {
                        q.Id.IsIn(follows.Data
                            .Where(f => f.CreatedUserId != entity.CreatedUserId)
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

                // Update state
                state.AddSent(FollowTypes.Category.Name);
                entity.AddOrUpdate(state);

                // Persist state
                await _entityStore.UpdateAsync(entity);
                
            });

            return Task.FromResult(entity);

        }

        Task<TEntity> SendNotificationsForAllCategories(TEntity entity)
        {
            
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            // No need to send notifications for hidden entities
            if (entity.IsHidden())
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
                        q.Name.Equals(FollowTypes.AllCategories.Name);
                    })
                    .ToList();

                // No follows simply return
                if (follows?.Data == null)
                {
                    return;
                }
               
                // Get follow state for entity
                var state = entity.GetOrCreate<FollowState>();

                // Have category notifications already been sent for the entity?
                var follow = state.FollowsSent.FirstOrDefault(f =>
                    f.Name.Equals(FollowTypes.AllCategories.Name, StringComparison.InvariantCultureIgnoreCase));
                if (follow != null && follow.Sent)
                {
                    return;
                }
                
                // Get a collection of all users to notify
                // Exclude the author so they are not notified of there own posts
                var users = await _platoUserStore.QueryAsync()
                    .Select<UserQueryParams>(q =>
                    {
                        q.Id.IsIn(follows.Data
                            .Where(f => f.CreatedUserId != entity.CreatedUserId)
                            .Select(f => f.CreatedUserId)
                            .ToArray());
                    })
                    .ToList();

                // No follows simply return
                if (users?.Data == null)
                {
                    return;
                }
                
                // Send notifications
                foreach (var user in users.Data)
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

                // Update state
                state.AddSent(FollowTypes.AllCategories.Name);
                entity.AddOrUpdate(state);

                // Persist state
                await _entityStore.UpdateAsync(entity);

            });

            return Task.FromResult(entity);

        }
        
        #endregion

    }

}
