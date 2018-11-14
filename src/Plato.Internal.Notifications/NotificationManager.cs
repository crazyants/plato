using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Abstractions.Models;

namespace Plato.Internal.Notifications
{

    public class NotificationManager : INotificationManager
    {

        public NotificationManager()
        {

        }

        public Task<ICommandResult<INotification>> SendAsync(INotification notification)
        {
            throw new NotImplementedException();
        }

    }
}
