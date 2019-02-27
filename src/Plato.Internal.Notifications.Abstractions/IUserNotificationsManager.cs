using Plato.Internal.Abstractions;

namespace Plato.Internal.Notifications.Abstractions
{
    public interface IUserNotificationsManager<TNotification> : ICommandManager<TNotification> where TNotification : class
    {

    }
}
