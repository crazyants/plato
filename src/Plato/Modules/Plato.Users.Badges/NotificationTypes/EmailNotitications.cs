using System;
using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Users.Badges.NotificationTypes
{
    
    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification NewBadge =
            new EmailNotification("NewBadgeEmail", "New Badges",
                "Send me an email notification when I'm awarded a new badge.");
        
        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewBadge
            };

        }

        public IEnumerable<INotificationType> GetDefaultPermissions()
        {
            throw new NotImplementedException();
        }
     
    }

}
