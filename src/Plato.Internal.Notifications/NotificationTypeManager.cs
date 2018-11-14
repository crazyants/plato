using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Abstractions.Models;

namespace Plato.Internal.Notifications
{
    public class NotificationTypeManager<TNotificationType> : INotificationTypeManager<TNotificationType> where TNotificationType : class, INotificationType
    {

        private IEnumerable<TNotificationType> _notificationTypes;

        private readonly IEnumerable<INotificationTypeProvider<TNotificationType>> _providers;
        private readonly ILogger<NotificationTypeManager<TNotificationType>> _logger;

        public NotificationTypeManager(
            IEnumerable<INotificationTypeProvider<TNotificationType>> providers,
            ILogger<NotificationTypeManager<TNotificationType>> logger)
        {
            _providers = providers;
            _logger = logger;
        }

        public IEnumerable<TNotificationType> GetNotificationTypes()
        {

            if (_notificationTypes == null)
            {
                var notificationTypes = new List<TNotificationType>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        notificationTypes.AddRange(provider.GetNotificationTypes());
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"An exception occurred within the notification type provider. Please review your notification provider and try again. {e.Message}");
                        throw;
                    }
                }

                _notificationTypes = notificationTypes;
            }

            return _notificationTypes;

        }
        
        public IDictionary<string, IEnumerable<TNotificationType>> GetCategorizedNotificationTypes()
        {

            var output = new Dictionary<string, IEnumerable<TNotificationType>>();

            foreach (var provider in _providers)
            {

                var notificationTypes = provider.GetNotificationTypes();
                foreach (var notificationType in notificationTypes)
                {
                    var name = notificationType.Name;
                    var category = notificationType.Category;
                    var title = String.IsNullOrWhiteSpace(category) ?
                        name :
                        category;

                    if (output.ContainsKey(title))
                    {
                        output[title] = output[title].Concat(new[] { notificationType });
                    }
                    else
                    {
                        output.Add(title, new[] { notificationType });
                    }
                }
            }

            return output;
        }

    }
    
}
