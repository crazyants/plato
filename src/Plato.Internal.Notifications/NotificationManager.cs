using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Internal.Notifications
{

    public class NotificationManager<TModel> : INotificationManager<TModel> where TModel : class
    {
        
        private readonly IEnumerable<INotificationProvider<TModel>> _notificationProviders;
        private readonly ILogger<NotificationManager<TModel>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceCollection _applicationServices;

        public NotificationManager(
            IEnumerable<INotificationProvider<TModel>> notificationProviders, 
            ILogger<NotificationManager<TModel>> logger, 
            IServiceProvider serviceProvider, 
            IServiceCollection applicationServices)
        {
            _notificationProviders = notificationProviders;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _applicationServices = applicationServices;
        }
        
        public async Task<IEnumerable<ICommandResult<TModel>>> SendAsync(INotification notification, TModel model) 
        {

            // Clone services to expose to notification providers
            // Some notification may need these services as they run on a background thread
            //var clonedServices = _serviceProvider.CreateChildContainer(_applicationServices);

            // Create context for notification providers
            var context = new NotificationContext<TModel>()
            {
                Model = model,
                Notification = notification
            };

            // Invoke notification providers
            var results = new List<ICommandResult<TModel>>();
            foreach (var notificationProvider in _notificationProviders)
            {
                try
                {
                    var result = await notificationProvider.SendAsync(context);
                    if (result != null)
                    {
                        results.Add(result);
                    }
                }
                catch (Exception e)
                {
                    if (_logger.IsEnabled(LogLevel.Critical))
                    {
                        _logger.LogError(
                            $"An error occurred whilst invoking the SendAsync method within a notification provider for notification type '{notification.Type.Name}'. Error Message: {e.Message}");
                    }
                }
            }
            
            // Log results
            foreach (var result in results)
            {
                if (result.Succeeded)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"Notification '{notification.Type.Title}' Success!",
                            $"To: {notification.To.DisplayName}, Message: ");
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogCritical($"Notification '{notification.Type.Title}' Failed!",
                                $"To: {notification.To.DisplayName}, Message: {error.Description}");
                        }
                    }
                }

            }
            
            return results;

        }

    }

}
