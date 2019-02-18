using System.Collections.Generic;
using Plato.Internal.Models.Notifications;

namespace Plato.Internal.Notifications.Abstractions
{
 
    public class DefaultNotificationTypes
    {

        public string RoleName { get; set; }

        public IEnumerable<INotificationType> NotificationTypes { get; set; }

    }


}
