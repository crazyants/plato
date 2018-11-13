using System.Collections.Generic;
using Plato.Internal.Abstractions;

namespace Plato.Notifications.Models
{
    public class UserNotificationSettings : Serializable
    {

        public IEnumerable<NotificationType> NotificationTypes { get; set; }

    }
}
