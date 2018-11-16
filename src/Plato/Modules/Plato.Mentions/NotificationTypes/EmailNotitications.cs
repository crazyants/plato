using System;
using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Mentions.NotificationTypes
{
    
    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification NewMention =
            new EmailNotification("NewMentionEmail", "New Mentions",
                "Send me an email notification for each new @mention.");
        
        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewMention
            };

        }

        public IEnumerable<INotificationType> GetDefaultPermissions()
        {
            throw new NotImplementedException();
        }
     
    }

}
