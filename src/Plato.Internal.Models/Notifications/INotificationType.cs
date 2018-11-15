using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Models.Notifications
{

    public interface INotificationType
    {
        string Id { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string Category { get; }

        Func<INotificationContext, Task<ICommandResult<Notification>>> Sender { get; set; }

        //Task<ICommandResult<Notification>> Send<T>(INotificationContext context) where T : class;

    }

}
