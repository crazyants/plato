using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Notifications.Abstractions
{
    public interface IUserNotificationTypeDefaults
    {
        IEnumerable<UserNotificationType> GetUserNotificationTypesWithDefaults(IUser user);

    }
}
