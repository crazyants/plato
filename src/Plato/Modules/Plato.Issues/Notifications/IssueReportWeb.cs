using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Issues.Models;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Issues.NotificationTypes;
using Plato.Entities;
using Plato.Entities.Models;

namespace Plato.Issues.Notifications
{
    public class IssueReportWeb : INotificationProvider<ReportSubmission<Issue>>
    {

        private readonly IUserNotificationsManager<UserNotification> _userNotificationManager;
        private readonly ICapturedRouterUrlHelper _urlHelper;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public IssueReportWeb(
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

        public async Task<ICommandResult<ReportSubmission<Issue>>> SendAsync(INotificationContext<ReportSubmission<Issue>> context)
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
            if (!context.Notification.Type.Name.Equals(WebNotifications.IssueReport.Name, StringComparison.Ordinal))
            {
                return null;
            }
            
            // Create result
            var result = new CommandResult<ReportSubmission<Issue>>();
            
            var baseUri = await _urlHelper.GetBaseUrlAsync();
            var url = _urlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
            {
                ["area"] = "Plato.Issues",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = context.Model.What.Id,
                ["opts.alias"] = context.Model.What.Alias
            });

            // Get reason given text
            var reasonText = S["Issue Reported"];
            if (ReportReasons.Reasons.ContainsKey(context.Model.Why))
            {
                reasonText = S[ReportReasons.Reasons[context.Model.Why]];
            }
            
            //// Build notification
            var userNotification = new UserNotification()
            {
                NotificationName = context.Notification.Type.Name,
                UserId = context.Notification.To.Id,
                Title = reasonText.Value,
                Message = S["An issue has been reported!"],
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
