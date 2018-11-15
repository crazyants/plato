using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Internal.Notifications
{

    public class NotificationManager : INotificationManager
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceCollection _applicationServices;

        public NotificationManager(
            IServiceProvider serviceProvider,
            IServiceCollection applicationServices)
        {
            _serviceProvider = serviceProvider;
            _applicationServices = applicationServices;
        }

        public async Task<ICommandResult<INotification>> SendAsync<T>(INotification notification) where T : class
        {
            var clonedServices = _serviceProvider.CreateChildContainer(_applicationServices);
            var context = new NotificationContext(clonedServices.BuildServiceProvider());
            return await notification.NotificationType.Sender(context);
        }

    }

}
