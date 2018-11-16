using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Notifications;

namespace Plato.Internal.Notifications.Abstractions
{

    public interface INotificationManager<TModel> where TModel : class
    {

        Task<IEnumerable<ICommandResult<TModel>>> SendAsync(INotification notification, TModel model);

    }
    
}
