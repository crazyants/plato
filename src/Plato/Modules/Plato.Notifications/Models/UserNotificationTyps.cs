using System.Collections.Generic;
using Plato.Internal.Abstractions;

namespace Plato.Notifications.Models
{
    public class UserNotificationTypes : Serializable
    {

        public IEnumerable<UserNotificationType> NotificationTypes { get; set; }

    }

}
