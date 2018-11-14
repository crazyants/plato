using System.Collections.Generic;
using Plato.Internal.Abstractions;

namespace Plato.Notifications.Models
{
    public class UserNotificationTyps : Serializable
    {

        public IEnumerable<UserNotificationType> NotificationTypes { get; set; }

    }
}
