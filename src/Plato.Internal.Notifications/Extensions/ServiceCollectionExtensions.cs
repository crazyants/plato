using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Abstractions.Models;


namespace Plato.Internal.Notifications.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoNotifications(
            this IServiceCollection services)
        {
            
            // Type Manager
            services.TryAddScoped<INotificationTypeManager<NotificationType>, NotificationTypeManager<NotificationType>>();

            // Notification Manager
            services.TryAddScoped<INotificationManager, NotificationManager>();

            return services;

        }


    }
}
