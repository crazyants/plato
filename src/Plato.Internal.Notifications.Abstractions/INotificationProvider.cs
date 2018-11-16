using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Notifications;

namespace Plato.Internal.Notifications.Abstractions
{
    public interface INotificationProvider<TModel> where TModel : class
    {

        Task<ICommandResult<TModel>> SendAsync(INotificationContext<TModel> context);

    }
    
}
