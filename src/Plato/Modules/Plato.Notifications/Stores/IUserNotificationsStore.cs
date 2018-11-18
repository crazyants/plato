using Plato.Internal.Stores.Abstractions;

namespace Plato.Notifications.Stores
{

    public interface IUserNotificationsStore<TModel> : IStore<TModel> where TModel : class
    {
    }

}
