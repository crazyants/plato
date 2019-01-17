using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Discuss.Labels.Follow.NotificationTypes
{

    public class EmailNotifications : INotificationTypeProvider
    {
        
        public static readonly EmailNotification NewLabel =
            new EmailNotification("NewLabelEmail", "New Labels", "Send me an email notification  for each new post associated with a label I'm following.");

        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewLabel,
            };
        }

        public IEnumerable<INotificationType> GetDefaultNotificationTypes()
        {
            return new[]
            {
                NewLabel,
            };
        }
    }

}
