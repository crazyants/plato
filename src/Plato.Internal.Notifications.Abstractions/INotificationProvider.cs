using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Notifications;

namespace Plato.Internal.Notifications.Abstractions
{
    public interface INotificationProvider
    {

        Task<ICommandResultBase> SendAsync<T>(INotificationContext<T> context) where T : class;

    }
    
}
