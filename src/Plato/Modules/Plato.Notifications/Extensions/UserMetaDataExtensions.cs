using System;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Notifications.Models;

namespace Plato.Notifications.Extensions
{
    public static class UserMetaDataExtensions
    {

        /// <summary>
        ///  Returns a boolean to indicate if the user has explicitly opted-in to a notification type.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="notificationType"></param>
        /// <returns></returns>
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
            
            return false;

        }

        /// <summary>
        /// Returns a boolean to indicate if the user has explicitly opted-in to a notification type.
        /// Use the features default notification types if the user has not saved the notification type.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userNotificationTypeDefaults"></param>
        /// <param name="notificationType"></param>
        /// <returns></returns>
        public static bool NotificationEnabled(
            this IUserMetaData<UserData> user,
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            INotificationType notificationType)
        {
            
            foreach (var userNotificationType in userNotificationTypeDefaults.GetUserNotificationTypesWithDefaults(user))
            {
                if (userNotificationType.Name.Equals(notificationType.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return userNotificationType.Enabled;
                }
            }

            return false;
            
        }

    }

}
