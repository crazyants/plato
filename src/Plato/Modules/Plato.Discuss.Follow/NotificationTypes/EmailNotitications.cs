using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Discuss.Follow.NotificationTypes
{

    public class EmailNotifications : INotificationTypeProvider
    {
        
        public static readonly EmailNotification NewReply =
            new EmailNotification("NewReplyEmail", "New Replies", "Send me an email notification for each new reply within topics I'm following.");

        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewReply,
            };
        }

        public IEnumerable<INotificationType> GetDefaultNotificationTypes()
        {
            return new[]
            {
                NewReply,
            };
        }
    }

}
