using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Notifications.Services;

namespace Plato.Users.Badges.Services
{
    public class DummyUserNotificationTypeDefaults : IUserNotificationTypeDefaults
    {

        private readonly INotificationTypeManager _notificationTypeManager;

        public DummyUserNotificationTypeDefaults(INotificationTypeManager notificationTypeManager)
        {
            _notificationTypeManager = notificationTypeManager;
        }

        public IEnumerable<UserNotificationType> GetUserNotificationTypesWithDefaults(IUserMetaData<UserData> user)
        {
            var defaultUserNotificationTypes = new List<UserNotificationType>();
            foreach (var notificationType in _notificationTypeManager.GetDefaultNotificationTypes())
            {
                defaultUserNotificationTypes.Add(new UserNotificationType(notificationType.Name));
            }

            return defaultUserNotificationTypes;

        }
    }
}
