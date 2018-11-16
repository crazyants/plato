using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Internal.Notifications
{

    public class NotificationManager : INotificationManager
    {
        

        private readonly IEnumerable<INotificationProvider> _notificationProviders;
        private readonly ILogger<NotificationManager> _logger;

        public NotificationManager(
            IEnumerable<INotificationProvider> notificationProviders, 
            ILogger<NotificationManager> logger)
        {

            _notificationProviders = notificationProviders;
            _logger = logger;
        }

      
        public async Task<ICommandResult<T>> SendAsync<T>(INotification notification) where T : class
        {
            
            var context = new NotificationContext<T>()
            {
                Notification = notification
            };

            // Iterate notification providers attempting to send
            foreach (var notificationProvider in _notificationProviders)
            {
                await notificationProvider.SendAsync<T>(context);
            }

            //var result = await notification.NotificationType.Sender(context);
            return new CommandResult<T>();
        }

    }

}
