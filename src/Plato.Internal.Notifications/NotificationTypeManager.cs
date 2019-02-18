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
        
        private IEnumerable<DefaultNotificationTypes> _notificationTypes;
        private IEnumerable<DefaultNotificationTypes> _defaultNotificationTypes;

        private readonly IEnumerable<INotificationTypeProvider> _providers;
        private readonly ILogger<NotificationTypeManager> _logger;

        public NotificationTypeManager(
            IEnumerable<INotificationTypeProvider> providers,
            ILogger<NotificationTypeManager> logger)
        {
            _providers = providers;
            _logger = logger;
        }
        
        public IEnumerable<INotificationType> GetNotificationTypes(IEnumerable<string> roleNames)
        {

            if (_notificationTypes == null)
            {
                var notificationTypes = new List<DefaultNotificationTypes>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        var types = provider.GetNotificationTypes();
                        if (types != null)
                        {
                            notificationTypes.AddRange(types);
                        }
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

            // Collapse to a distinct list of notification
            // types matching the supplied roleNames
            return _notificationTypes
                .Where(n => roleNames.Contains(n.RoleName))
                .SelectMany(n => n.NotificationTypes)
                .Distinct()
                .ToList();

        }
        
        public IEnumerable<INotificationType> GetDefaultNotificationTypes(IEnumerable<string> roleNames)
        {
            if (_defaultNotificationTypes == null)
            {
                var notificationTypes = new List<DefaultNotificationTypes>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        var defaultNotificationTypes = provider.GetDefaultNotificationTypes();
                        if (defaultNotificationTypes != null)
                        {
                            notificationTypes.AddRange(defaultNotificationTypes);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"An exception occurred within the notification type provider. Please review your notification provider and try again. {e.Message}");
                        throw;
                    }
                }

                _defaultNotificationTypes = notificationTypes;
            }

            // Collapse to a distinct list of notification
            // types matching the supplied roleNames
            return _defaultNotificationTypes
                .Where(n => roleNames.Contains(n.RoleName))
                .SelectMany(n => n.NotificationTypes)
                .Distinct()
                .ToList();
        }
        
        public IDictionary<string, IEnumerable<INotificationType>> GetCategorizedNotificationTypes(IEnumerable<string> roleNames)
        {
            var output = new Dictionary<string, IEnumerable<INotificationType>>();
            var notificationTypes = GetNotificationTypes(roleNames);
            foreach (var notificationType in notificationTypes)
            {
                var name = notificationType.Title;
                var category = notificationType.Category;
                var title = String.IsNullOrWhiteSpace(category) ? name : category;

                if (output.ContainsKey(title))
                {
                    output[title] = output[title].Concat(new[] {notificationType});
                }
                else
                {
                    output.Add(title, new[] {notificationType});
                }
            }
            
            return output;

        }

    }

}
