using System.Collections.Generic;
using Plato.Internal.Abstractions;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Notifications.Models
{
 
    public class UserNotificationTypes : Serializable, IUserNotificationTypes
    {

        public IEnumerable<IUserNotificationType> NotificationTypes { get; set; }

    }

}
