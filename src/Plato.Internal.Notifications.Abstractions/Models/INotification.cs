namespace Plato.Internal.Notifications.Abstractions.Models
{

    public interface INotification
    {
        INotificationType NotificationType { get; }
    }

}
