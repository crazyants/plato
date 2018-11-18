using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Discuss.Models;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Discuss.Mentions.NotificationTypes;
using Plato.Notifications.Models;
using Plato.Notifications.Services;

namespace Plato.Discuss.Mentions.Notifications
{

    public class NewMentionWeb : INotificationProvider<Topic>
    {

        private readonly IContextFacade _contextFacade;
        private readonly ILocaleStore _localeStore;
        private readonly IEmailManager _emailManager;
        private readonly IUserNotificationsManager<UserNotification> _userNotificationManager;

        public NewMentionWeb(
            IContextFacade contextFacade,
            ILocaleStore localeStore,
            IEmailManager emailManager, 
            IUserNotificationsManager<UserNotification> userNotificationManager)
        {
            _contextFacade = contextFacade;
            _localeStore = localeStore;
            _emailManager = emailManager;
            _userNotificationManager = userNotificationManager;
        }

        public async Task<ICommandResult<Topic>> SendAsync(INotificationContext<Topic> context)
        {

            // Validate
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Notification == null)
            {
                throw new ArgumentNullException(nameof(context.Notification));
            }

            if (context.Notification.Type == null)
            {
                throw new ArgumentNullException(nameof(context.Notification.Type));
            }

            if (context.Notification.To == null)
            {
                throw new ArgumentNullException(nameof(context.Notification.To));
            }

            // Ensure correct notification provider
            if (context.Notification.Type.Id != WebNotifications.NewMention.Id)
            {
                return null;
            }

            // Create result
            var result = new CommandResult<Topic>();
            
            // Build topic url
            var baseUrl = await _contextFacade.GetBaseUrlAsync();
            var topicUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["Area"] = "Plato.Discuss",
                ["Controller"] = "Home",
                ["Action"] = "Topic",
                ["Id"] = context.Model.Id,
                ["Alias"] = context.Model.Alias
            });

            // Build user notification
            var userNotification = new UserNotification()
            {
                NotificationName = context.Notification.Type.Id,
                UserId = context.Notification.To.Id,
                Title = "New Mention",
                Message = "You've been mentioned by " + context.Model.CreatedBy.DisplayName,
                Url = topicUrl
            };

            var userNotificationResult = await _userNotificationManager.CreateAsync(userNotification);
            if (userNotificationResult.Succeeded)
            {
                return result.Success(context.Model);
            }

            return result.Failed(userNotificationResult.Errors?.ToArray());
            
        }

    }
    
}
