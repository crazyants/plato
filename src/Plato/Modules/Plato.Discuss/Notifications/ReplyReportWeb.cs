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
using Plato.Discuss.NotificationTypes;
using Plato.Entities;
using Plato.Entities.Models;
using Plato.Entities.Stores;

namespace Plato.Discuss.Notifications
{
    public class ReplyReportWeb : INotificationProvider<ReportSubmission<Reply>>
    {

        private readonly IUserNotificationsManager<UserNotification> _userNotificationManager;
        private readonly ICapturedRouterUrlHelper _urlHelper;
        private readonly IEntityStore<Topic> _entityStore;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public ReplyReportWeb(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            ICapturedRouterUrlHelper urlHelper,
            IUserNotificationsManager<UserNotification> userNotificationManager,
            IEntityStore<Topic> entityStore)
        {
            _urlHelper = urlHelper;
            _userNotificationManager = userNotificationManager;
            _entityStore = entityStore;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        public async Task<ICommandResult<ReportSubmission<Reply>>> SendAsync(INotificationContext<ReportSubmission<Reply>> context)
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
            if (!context.Notification.Type.Name.Equals(WebNotifications.ReplyReport.Name, StringComparison.Ordinal))
            {
                return null;
            }

            var entity = await _entityStore.GetByIdAsync(context.Model.What.EntityId);
            if (entity == null)
            {
                return null;
            }

            // Create result
            var result = new CommandResult<ReportSubmission<Reply>>();
            
            var baseUri = await _urlHelper.GetBaseUrlAsync();
            var url = _urlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.id"] = entity.Alias,
                ["opts.replyId"] = context.Model.What.Id
            });

            //// Build notification
            var userNotification = new UserNotification()
            {
                NotificationName = context.Notification.Type.Name,
                UserId = context.Notification.To.Id,
                Title = S[ReportReasons.Reasons[context.Model.Why]].Value,
                Message = S["A reply has been reported!"],
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
