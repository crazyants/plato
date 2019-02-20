using System;
using System.Collections.Generic;
using System.Linq;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Notifications.Models;

namespace Plato.Notifications.Services
{
    
    public class UserNotificationTypeDefaults : IUserNotificationTypeDefaults
    {

        private readonly INotificationTypeManager _notificationTypeManager;

        public UserNotificationTypeDefaults(
            INotificationTypeManager notificationTypeManager)
        {
            _notificationTypeManager = notificationTypeManager;
        }

        public IEnumerable<UserNotificationType> GetUserNotificationTypesWithDefaults(IUser user)
        {

            // Build a list of all available default notification types
            // These should be enabled by default unless the user has explicitly disabled it
            var defaultUserNotificationTypes = new List<UserNotificationType>();
            var defaultNotificationTypes = _notificationTypeManager.GetDefaultNotificationTypes(user.RoleNames);
            if (defaultNotificationTypes != null)
            {
                foreach (var defaultNotificationType in defaultNotificationTypes)
                {
                    defaultUserNotificationTypes.Add(new UserNotificationType(defaultNotificationType.Name));
                }
            }

            // Get all saved notification types for the given user
            var userNotificationSettings = user.GetOrCreate<UserNotificationTypes>();
            
            // We have previously saved settings
            var output = new List<UserNotificationType>();
            if (userNotificationSettings.NotificationTypes != null)
            {

                // Add all user specified notification types
                output.AddRange(userNotificationSettings.NotificationTypes);

                // Loop through all default notification types to see if the user has previously saved
                // a value for that notification type, if no value have been previously saved
                // ensure the default notification type if available is added to our list of enabled types
                foreach (var userNotification in defaultUserNotificationTypes)
                {
                    var foundNotification = output.FirstOrDefault(n =>
                        n.Name.Equals(userNotification.Name, StringComparison.OrdinalIgnoreCase));
                    if (foundNotification == null)
                    {
                        output.Add(userNotification);
                    }
                }
            }
            else
            {
                // If we don't have any notification types ensure we enable all defaults
                output.AddRange(defaultUserNotificationTypes);
            }

            return output;

        }

    }

}
