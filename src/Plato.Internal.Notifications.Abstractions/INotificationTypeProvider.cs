using System.Collections.Generic;
using Plato.Internal.Notifications.Abstractions.Models;

namespace Plato.Internal.Notifications.Abstractions
{

    public interface INotificationTypeProvider<out TNotificationType> where TNotificationType : class, INotificationType
    {
        IEnumerable<TNotificationType> GetNotificationTypes();

        IEnumerable<TNotificationType> GetDefaultPermissions();

    }

}
