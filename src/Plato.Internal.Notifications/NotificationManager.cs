using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Internal.Notifications
{

    public class NotificationManager<TModel> : INotificationManager<TModel> where TModel : class
    {
        
        private readonly IEnumerable<INotificationProvider<TModel>> _notificationProviders;
        private readonly ILogger<NotificationManager<TModel>> _logger;

        public NotificationManager(
            IEnumerable<INotificationProvider<TModel>> notificationProviders, 
            ILogger<NotificationManager<TModel>> logger)
        {
            _notificationProviders = notificationProviders;
            _logger = logger;
        }
        
        public async Task<IEnumerable<ICommandResult<TModel>>> SendAsync(INotification notification, TModel model) 
        {
            
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
                var result = await notificationProvider.SendAsync(context);
                if (result != null)
                {
                    results.Add(result);
                }
                
            }
            
            // Log results
            foreach (var result in results)
            {
                if (result.Succeeded)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"Notification '{notification.Type.Name}' Success!",
                            $"To: {notification.To.DisplayName}, Message: ");
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogCritical($"Notification '{notification.Type.Name}' Failed!",
                                $"To: {notification.To.DisplayName}, Message: {error.Description}");
                        }
                    }
                }

            }
            
            return results;

        }

    }

}
