using System.Collections.Generic;

namespace Plato.Internal.Notifications.Abstractions
{
    
    public interface IUserNotificationTypes
    {
        IEnumerable<UserNotificationType> NotificationTypes { get; set; }
    }
    
}
