using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Discuss.Tags.Follow.NotificationTypes;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Follows.Stores;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Tasks.Abstractions;
using Plato.Notifications.Extensions;
using Plato.Tags.Models;

namespace Plato.Discuss.Tags.Follow.Subscribers
{
    public class EntityTagSubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IBroker _broker;
        private readonly IEntityStore<TEntity> _entityStore;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly INotificationManager<TEntity> _notificationManager;
        private readonly IFollowStore<Follows.Models.Follow> _followStore;
        private readonly IUserDataMerger _userDataMerger;
     
        public EntityTagSubscriber(
            IBroker broker,
            IDeferredTaskManager deferredTaskManager,
            INotificationManager<TEntity> notificationManager,
            IFollowStore<Follows.Models.Follow> followStore,
            IUserDataMerger userDataMerger,
            IEntityStore<TEntity> entityStore)
        {
            _broker = broker;
            _deferredTaskManager = deferredTaskManager;
            _notificationManager = notificationManager;
            _followStore = followStore;
            _userDataMerger = userDataMerger;
            _entityStore = entityStore;
        }

        #region "Implementation"

        public void Subscribe()
        {
            // Created
            _broker.Sub<EntityTag>(new MessageOptions()
            {
                Key = "EntityTagCreated",
                Order = short.MaxValue
            }, async message => await EntityTagCreated(message.What));

            // Updated
            _broker.Sub<EntityTag>(new MessageOptions()
            {
                Key = "EntityTagUpdated",
                Order = short.MaxValue
            }, async message => await EntityTagUpdated(message.What));

        }

        public void Unsubscribe()
        {
            // Created
            _broker.Unsub<EntityTag>(new MessageOptions()
            {
                Key = "EntityTagCreated",
                Order = short.MaxValue
            }, async message => await EntityTagCreated(message.What));

            // Updated
            _broker.Unsub<EntityTag>(new MessageOptions()
            {
                Key = "EntityTagUpdated",
                Order = short.MaxValue
            }, async message => await EntityTagUpdated(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<EntityTag> EntityTagCreated(EntityTag entityTag)
        {

            if (entityTag.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityTag.EntityId));
            }

            var entity = await _entityStore.GetByIdAsync(entityTag.EntityId);
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            // No need to send notifications for entities flagged as private
            if (entity.IsPrivate)
            {
                return entityTag;
            }

            // No need to send notifications for entities flagged as spam
            if (entity.IsSpam)
            {
                return entityTag;
            }

            // Defer notifications to first available thread pool thread
            _deferredTaskManager.AddTask(async context =>
            {

                // Get all follows for tag
                var follows = await _followStore.QueryAsync()
                    .Select<FollowQueryParams>(q =>
                    {
                        q.ThingId.Equals(entityTag.TagId);
                        q.Name.Equals(FollowTypes.Tag.Name);
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
                    if (user.NotificationEnabled(EmailNotifications.NewTag))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.NewTag)
                        {
                            To = user,
                        }, entity);
                    }

                    // Web notifications
                    if (user.NotificationEnabled(WebNotifications.NewTag))
                    {
                        await _notificationManager.SendAsync(new Notification(WebNotifications.NewTag)
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

            return entityTag;


        }

        Task<EntityTag> EntityTagUpdated(EntityTag entityTag)
        {
            // Tag notifications are not triggered for entity updates
            // This could possibly be implemented at a later stage
            return Task.FromResult(entityTag);
        }
        
        #endregion

    }

}
