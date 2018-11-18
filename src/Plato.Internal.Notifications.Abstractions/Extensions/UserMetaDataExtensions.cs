using System;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Notifications.Abstractions.Extensions
{

    public static class UserExtensions
    {

        public static bool NotificationEnabled<TStore>(
            this IUserMetaData<UserData> user,
            INotificationType notificationType) where TStore : class, IUserNotificationTypes
        {

            var settings = user.GetOrCreate<TStore>();
            var notificationTypes = settings?.NotificationTypes;
            if (notificationTypes != null)
            {
                foreach (var localNotificationType in notificationTypes)
                {
                    if (localNotificationType.Id.Equals(notificationType.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;

        }

    }

}
