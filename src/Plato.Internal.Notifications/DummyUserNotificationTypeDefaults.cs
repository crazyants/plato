using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Internal.Notifications
{
    public class DummyUserNotificationTypeDefaults : IUserNotificationTypeDefaults
    {

        public IEnumerable<UserNotificationType> GetUserNotificationTypesWithDefaults(IUserMetaData<UserData> user)
        {
            return new List<UserNotificationType>();
        }
    }

}
