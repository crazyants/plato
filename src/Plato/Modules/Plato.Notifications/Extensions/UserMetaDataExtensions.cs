using System;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Notifications.Models;

namespace Plato.Notifications.Extensions
{

    public static class UserMetaDataExtensions
    {

        public static bool NotificationEnabled(
            this IUserMetaData<UserData> user,
            INotificationType notificationType)
        {

            var settings = user.GetOrCreate<UserNotificationTypes>();
            if (settings?.NotificationTypes != null)
            {
                foreach (var localNotificationType in settings?.NotificationTypes)
                {
                    if (!String.IsNullOrEmpty(localNotificationType.Name))
                    {
                        if (localNotificationType.Name.Equals(notificationType.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;

        }

    }

}
