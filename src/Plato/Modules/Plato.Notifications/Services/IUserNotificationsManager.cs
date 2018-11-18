using Plato.Internal.Abstractions;

namespace Plato.Notifications.Services
{

    public interface IUserNotificationsManager<TEntityMention> : ICommandManager<TEntityMention> where TEntityMention : class
    {

    }
    
}
