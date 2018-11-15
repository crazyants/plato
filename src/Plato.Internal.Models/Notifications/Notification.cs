using Plato.Internal.Models.Users;

namespace Plato.Internal.Models.Notifications
{
    
    public class Notification : INotification
    {

        public IUser To { get; set; }

        public INotificationType NotificationType { get; set; }


    }

}
