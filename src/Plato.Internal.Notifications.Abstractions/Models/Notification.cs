namespace Plato.Internal.Notifications.Abstractions.Models
{
    
    public class Notification : INotification
    {
        public INotificationType NotificationType { get; }

        public Notification(INotificationType notificationType)
        {
            this.NotificationType = notificationType;
        }

    }

}
