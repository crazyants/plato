using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Users.Badges.NotificationTypes
{

    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification NewBadge =
            new WebNotification("NewBadgeWeb", "New Badges",
                "Show me a web notification when I'm awarded a new badge.");

        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewBadge
            };
        }

        public IEnumerable<INotificationType> GetDefaultPermissions()
        {
            return null;
        }

    }

}
