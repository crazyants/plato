using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Discuss.StopForumSpam.NotificationTypes;

namespace Plato.Discuss.StopForumSpam.Notifications
{
    public class TopicSpamWeb : INotificationProvider<Topic>
    {

        private readonly IUserNotificationsManager<UserNotification> _userNotificationManager;
        private readonly ICapturedRouterUrlHelper _urlHelper;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public TopicSpamWeb(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            ICapturedRouterUrlHelper urlHelper,
            IUserNotificationsManager<UserNotification> userNotificationManager)
        {
            _urlHelper = urlHelper;
            _userNotificationManager = userNotificationManager;

            T = htmlLocalizer;
            S = stringLocalizer;
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
            if (!context.Notification.Type.Name.Equals(WebNotifications.TopicSpam.Name, StringComparison.Ordinal))
            {
                return null;
            }
            
            // Create result
            var result = new CommandResult<Topic>();
            
            var baseUri = await _urlHelper.GetBaseUrlAsync();
            var url = _urlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
            {
                ["Area"] = "Plato.Discuss",
                ["Controller"] = "Home",
                ["Action"] = "Topic",
                ["Id"] = context.Model.Id,
                ["Alias"] = context.Model.Alias
            });

            //// Build notification
            var userNotification = new UserNotification()
            {
                NotificationName = context.Notification.Type.Name,
                UserId = context.Notification.To.Id,
                Title = S["Possible SPAM"].Value,
                Message = S["A topic has been detected as SPAM!"],
                Url = url,
                CreatedUserId = context.Notification.From?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            };

            // Create notification
            var userNotificationResult = await _userNotificationManager.CreateAsync(userNotification);
            if (userNotificationResult.Succeeded)
            {
                return result.Success(context.Model);
            }

            return result.Failed(userNotificationResult.Errors?.ToArray());
            
        }

    }

}
