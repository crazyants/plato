using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Discuss.Labels.Follow.NotificationTypes;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Follows.Stores;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Tasks.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Stores;
using Plato.Notifications.Extensions;
using Plato.Notifications.Services;

namespace Plato.Discuss.Labels.Follow.Subscribers
{
    public class EntityLabelSubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IBroker _broker;
        private readonly IEntityStore<TEntity> _entityStore;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly INotificationManager<TEntity> _notificationManager;
        private readonly IFollowStore<Follows.Models.Follow> _followStore;
        private readonly IUserDataMerger _userDataMerger;
        private readonly IEntityLabelStore<EntityLabel> _entityLabelStore;
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;

        public EntityLabelSubscriber(
            IBroker broker,
            IDeferredTaskManager deferredTaskManager,
            INotificationManager<TEntity> notificationManager,
            IFollowStore<Follows.Models.Follow> followStore,
            IUserDataMerger userDataMerger,
            IEntityLabelStore<EntityLabel> entityLabelStore,
            IEntityStore<TEntity> entityStore,
            IUserNotificationTypeDefaults userNotificationTypeDefaults)
        {
            _broker = broker;
            _deferredTaskManager = deferredTaskManager;
            _notificationManager = notificationManager;
            _followStore = followStore;
            _userDataMerger = userDataMerger;
            _entityLabelStore = entityLabelStore;
            _entityStore = entityStore;
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
        }

        #region "Implementation"

        public void Subscribe()
        {
            // Created
            _broker.Sub<EntityLabel>(new MessageOptions()
            {
                Key = "EntityLabelCreated",
                Order = short.MaxValue
            }, async message => await EntityLabelCreated(message.What));

            // Updated
            _broker.Sub<EntityLabel>(new MessageOptions()
            {
                Key = "EntityLabelUpdated",
                Order = short.MaxValue
            }, async message => await EntityLabelUpdated(message.What));

        }

        public void Unsubscribe()
        {
            // Created
            _broker.Unsub<EntityLabel>(new MessageOptions()
            {
                Key = "EntityLabelCreated",
                Order = short.MaxValue
            }, async message => await EntityLabelCreated(message.What));

            // Updated
            _broker.Unsub<EntityLabel>(new MessageOptions()
            {
                Key = "EntityLabelUpdated",
                Order = short.MaxValue
            }, async message => await EntityLabelUpdated(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<EntityLabel> EntityLabelCreated(EntityLabel entityLabel)
        {

            if (entityLabel.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityLabel.EntityId));
            }

            var entity = await _entityStore.GetByIdAsync(entityLabel.EntityId);
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            // No need to send notifications for entities flagged as private
            if (entity.IsPrivate)
            {
                return entityLabel;
            }

            // No need to send notifications for entities flagged as spam
            if (entity.IsSpam)
            {
                return entityLabel;
            }

            // Defer notifications to first available thread pool thread
            _deferredTaskManager.AddTask(async context =>
            {

                // Get all follows for found labels
                var follows = await _followStore.QueryAsync()
                    .Select<FollowQueryParams>(q =>
                    {
                        q.ThingId.Equals(entityLabel.LabelId);
                        q.Name.Equals(FollowTypes.Label.Name);
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
                var mergedUsers = await _userDataMerger.MergeAsync(users);

                // Send notifications
                foreach (var user in mergedUsers)
                {

                    // Email notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewLabel))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.NewLabel)
                        {
                            To = user,
                        }, entity);
                    }

                    // Web notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewLabel))
                    {
                        await _notificationManager.SendAsync(new Notification(WebNotifications.NewLabel)
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

            return entityLabel;


        }

        Task<EntityLabel> EntityLabelUpdated(EntityLabel entityLabel)
        {
            // Label notifications are not triggered for entity updates
            // This could possibly be implemented at a later stage
            return Task.FromResult(entityLabel);
        }
        
        #endregion

    }

}
