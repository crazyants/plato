using System.Collections.Generic;
using Plato.Internal.Notifications.Abstractions.Models;

namespace Plato.Internal.Notifications.Abstractions
{

    public interface INotificationTypeProvider
    {
        IEnumerable<INotificationType> GetNotificationTypes();

        IEnumerable<INotificationType> GetDefaultPermissions();

    }

}
