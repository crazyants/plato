using System;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Notifications.Models;

namespace Plato.Notifications.Extensions
{
    public static class UserExtensions
    {
        
        /// <summary>
        /// Returns a boolean to indicate if the user has explicitly opted-in to a notification type.
        /// Use the features default notification types if the user has not saved the notification type.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userNotificationTypeDefaults"></param>
        /// <param name="notificationType"></param>
        /// <returns></returns>
        //public static bool NotificationEnabled(
        //    this IUser user,
        //    IUserNotificationTypeDefaults userNotificationTypeDefaults,
        //    INotificationType notificationType)
        //{
            
        //    foreach (var userNotificationType in userNotificationTypeDefaults.GetUserNotificationTypesWithDefaults(user))
        //    {
        //        if (userNotificationType.Name.Equals(notificationType.Name, StringComparison.OrdinalIgnoreCase))
        //        {
        //            return userNotificationType.Enabled;
        //        }
        //    }

        //    return false;
            
        //}

    }

}
