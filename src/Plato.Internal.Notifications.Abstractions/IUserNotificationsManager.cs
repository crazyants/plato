using Plato.Internal.Abstractions;

namespace Plato.Internal.Notifications.Abstractions
{
    public interface IUserNotificationsManager<TNoficiation> : ICommandManager<TNoficiation> where TNoficiation : class
    {

    }
}
