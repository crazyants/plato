using System;
using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Users.StopForumSpam.NotificationTypes
{
    
    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification UserSpam =
            new EmailNotification("UserSpamEmail", "New Mentions",
                "Send me an email notification for each new @mention.");
        
        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                UserSpam
            };

        }

        public IEnumerable<INotificationType> GetDefaultNotificationTypes()
        {
            return new[]
            {
                UserSpam
            };
        }
     
    }

}
