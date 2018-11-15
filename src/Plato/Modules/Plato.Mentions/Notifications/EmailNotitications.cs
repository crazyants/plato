using System;
using System.Collections.Generic;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Abstractions.Models;

namespace Plato.Mentions.Notifications
{
    public class EmailNotifications : INotificationTypeProvider
    {
        
        public static readonly EmailNotification NewMention =
            new EmailNotification("NewMentionEmail", "New Mentions", "Show me a web notification for each new @mention.", "Web");
        
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
