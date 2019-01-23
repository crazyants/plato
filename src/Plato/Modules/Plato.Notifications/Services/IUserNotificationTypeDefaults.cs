using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;

namespace Plato.Notifications.Services
{
    public interface IUserNotificationTypeDefaults
    {
        IEnumerable<UserNotificationType> GetUserNotificationTypesWithDefaults(IUserMetaData<UserData> user);

    }

}
