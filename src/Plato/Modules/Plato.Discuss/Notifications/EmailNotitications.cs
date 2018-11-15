using System.Collections.Generic;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Abstractions.Models;

namespace Plato.Discuss.Notifications
{

    public class EmailNotifications : INotificationTypeProvider
    {
        
        public static readonly EmailNotification NewTopics =
            new EmailNotification("NewTopicsEmail", "New Topics", "Send me a email notification for each new topic.");
        
        public static readonly EmailNotification NewReplies =
            new EmailNotification("NewRepliesEmail", "New Replies", "Send me an email notification for each new reply.");

        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewTopics,
                NewReplies,
            };
        }

        public IEnumerable<INotificationType> GetDefaultPermissions()
        {
            return null;
        }

    }
    
}
