using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Discuss.Labels.Follow.NotificationTypes;
using Plato.Entities.Models;
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

namespace Plato.Discuss.Labels.Follow.Subscribers
{
    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IBroker _broker;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly INotificationManager<TEntity> _notificationManager;
        private readonly IFollowStore<Follows.Models.Follow> _followStore;
        private readonly IUserDataMerger _userDataMerger;
        private readonly IEntityLabelStore<EntityLabel> _entityLabelStore;

        public EntitySubscriber(
            IBroker broker,
            IDeferredTaskManager deferredTaskManager,
            INotificationManager<TEntity> notificationManager,
            IFollowStore<Follows.Models.Follow> followStore,
            IUserDataMerger userDataMerger,
            IEntityLabelStore<EntityLabel> entityLabelStore)
        {
            _broker = broker;
            _deferredTaskManager = deferredTaskManager;
            _notificationManager = notificationManager;
            _followStore = followStore;
            _userDataMerger = userDataMerger;
            _entityLabelStore = entityLabelStore;
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

        Task<TEntity> EntityCreated(TEntity entity)
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
            _deferredTaskManager.ExecuteAsync(async context =>
            {

                var entityLabels = await _entityLabelStore.GetByEntityId(entity.Id);
                
                // Get all follows for labels associated with entity
                var follows = await _followStore.QueryAsync()
                    .Select<FollowQueryParams>(q =>
                    {
                        if (entityLabels != null)
                        {
                            q.ThingId.IsIn(entityLabels.Select(l => l.LabelId).ToArray());
                        }
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
                    if (user.NotificationEnabled(EmailNotifications.NewLabel))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.NewLabel)
                        {
                            To = user,
                        }, entity);
                    }

                    // Web notifications
                    if (user.NotificationEnabled(WebNotifications.NewLabel))
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

            return Task.FromResult(entity);


        }

        Task<TEntity> EntityUpdated(TEntity entity)
        {
            // Label notifications are not triggered for entity updates
            // This could possibly be implemented at a later stage
            return Task.FromResult(entity);
        }
        
        #endregion

    }

}
