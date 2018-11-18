using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Users.Notifications.ViewModels
{

    public class EditNotificationsViewModel
    {

        public int Id { get; set; }

        public IDictionary<string, IEnumerable<INotificationType>> CategorizedNotificationTypes { get; set; }

        public IEnumerable<UserNotificationType> EnabledNotificationTypes { get; set; }
        
    }

}
