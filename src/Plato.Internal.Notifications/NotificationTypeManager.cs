using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Internal.Notifications
{
    public class NotificationTypeManager : INotificationTypeManager
    {

        private IEnumerable<INotificationType> _notificationTypes;

        private readonly IEnumerable<INotificationTypeProvider> _providers;
        private readonly ILogger<NotificationTypeManager> _logger;

        public NotificationTypeManager(
            IEnumerable<INotificationTypeProvider> providers,
            ILogger<NotificationTypeManager> logger)
        {
            _providers = providers;
            _logger = logger;
        }

        public IEnumerable<INotificationType> GetNotificationTypes()
        {

            if (_notificationTypes == null)
            {
                var notificationTypes = new List<INotificationType>();
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
        
        public IDictionary<string, IEnumerable<INotificationType>> GetCategorizedNotificationTypes()
        {

            var output = new Dictionary<string, IEnumerable<INotificationType>>();

            foreach (var provider in _providers)
            {

                var notificationTypes = provider.GetNotificationTypes();
                foreach (var notificationType in notificationTypes)
                {
                    var name = notificationType.Title;
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
