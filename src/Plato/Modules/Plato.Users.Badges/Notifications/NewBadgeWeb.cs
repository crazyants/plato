using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Badges.Models;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Hosting.Web;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Notifications.Models;
using Plato.Notifications.Services;
using Plato.Users.Badges.NotificationTypes;


namespace Plato.Users.Badges.Notifications
{

    public class NewBadgeWeb : INotificationProvider<Badge>
    {

        private readonly IContextFacade _contextFacade;
        private readonly IUserNotificationsManager<UserNotification> _userNotificationManager;
  
        private readonly ICapturedRouterUrlHelper _urlHelper;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public NewBadgeWeb(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IContextFacade contextFacade,
            IUserNotificationsManager<UserNotification> userNotificationManager,
            ICapturedRouterUrlHelper urlHelper)
        {
            _contextFacade = contextFacade;
            _userNotificationManager = userNotificationManager;
            _urlHelper = urlHelper;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<ICommandResult<Badge>> SendAsync(INotificationContext<Badge> context)
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
            if (!context.Notification.Type.Name.Equals(WebNotifications.NewBadge.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<Badge>();
            
            //var url = _urlHelper.Link()
            var baseUri = await _urlHelper.GetBaseUrlAsync();
            var url = _urlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
            {
                ["Area"] = "Plato.Users.Badges",
                ["Controller"] = "Profile",
                ["Action"] = "Index",
                ["Id"] = context.Notification.To.Id,
                ["Alias"] = context.Notification.To.Alias
            });

            //// Build notification
            var userNotification = new UserNotification()
            {
                NotificationName = context.Notification.Type.Name,
                UserId = context.Notification.To.Id,
                Title = S["New Badge"].Value,
                Message = $"{S["You've earned the"].Value} '{context.Model.Name}' {S[" badge"].Value}",
                Url = url
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
