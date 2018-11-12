using System.Collections.Generic;
using Plato.Notifications.Models;

namespace Plato.Notifications.Services
{

    public interface INotificationTypeProvider<out TNotificationType> where TNotificationType : class, INotificationType
    {
        IEnumerable<TNotificationType> GetNotificationTypes();

        IEnumerable<TNotificationType> GetDefaultPermissions();

    }

}
