using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Notifications.Abstractions.Models;

namespace Plato.Internal.Notifications.Abstractions
{

    public interface INotificationManager
    {

        Task<ICommandResult<INotification>> SendAsync(INotification notification);

    }
    
}
