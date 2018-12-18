using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Notifications.Abstractions;


namespace Plato.Internal.Notifications.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoNotifications(
            this IServiceCollection services)
        {
            
            // Type Manager
            services.TryAddScoped<INotificationTypeManager, NotificationTypeManager>();

            // Dummy user notification implementation
            services.TryAddScoped<IUserNotificationsManager<UserNotification>, DummyUserNotificationsManager>();

            return services;

        }


    }
}
