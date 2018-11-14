namespace Plato.Internal.Notifications.Abstractions.Models
{
    
    public class Notification : INotification
    {
        public NotificationType NotificationType { get; }

        public Notification(NotificationType notificationType)
        {
            this.NotificationType = notificationType;
        }

    }

}
