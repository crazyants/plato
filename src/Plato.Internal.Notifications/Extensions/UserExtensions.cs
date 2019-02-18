using System;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Internal.Notifications.Extensions
{
    public static class UserExtensions
    {

        public static bool NotificationEnabled(
            this IUser user,
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
