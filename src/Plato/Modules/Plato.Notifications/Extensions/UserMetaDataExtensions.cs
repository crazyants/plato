using System;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Notifications.Models;
using Plato.Notifications.Services;

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
                            return localNotificationType.Enabled;
                        }
                    }
                }
            }

            // Always allow notification if user has not explicitly disabled it
            return true;

        }
        public static bool NotificationEnabled(
            this IUserMetaData<UserData> user,
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            INotificationType notificationType)
        {

            var userNotificationTypes = userNotificationTypeDefaults.GetUserNotificationTypesWithDefaults(user);
            foreach (var userNotificationType in userNotificationTypes)
            {
                if (userNotificationType.Name.Equals(notificationType.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return userNotificationType.Enabled;
                }
            }

            return false;

            //var settings = user.GetOrCreate<UserNotificationTypes>();
            //if (settings?.NotificationTypes != null)
            //{
            //    foreach (var localNotificationType in settings?.NotificationTypes)
            //    {
            //        if (!String.IsNullOrEmpty(localNotificationType.Name))
            //        {
            //            if (localNotificationType.Name.Equals(notificationType.Name, StringComparison.OrdinalIgnoreCase))
            //            {
            //                return localNotificationType.Enabled;
            //            }
            //        }
            //    }
            //}

            // Always allow notification if user has not explicitly disabled it
            return true;

        }

    }

}
