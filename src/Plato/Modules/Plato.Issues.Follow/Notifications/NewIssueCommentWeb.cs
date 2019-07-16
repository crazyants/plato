using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Localization;
using Plato.Issues.Follow.NotificationTypes;
using Plato.Issues.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Issues.Follow.Notifications
{

    public class NewIssueCommentWeb : INotificationProvider<Comment>
    {
        
        private readonly IUserNotificationsManager<UserNotification> _userNotificationManager;
        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;
        private readonly IEntityStore<Issue> _entityStore;
     
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public NewIssueCommentWeb(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IUserNotificationsManager<UserNotification> userNotificationManager,
            ICapturedRouterUrlHelper capturedRouterUrlHelper,
            IEntityStore<Issue> entityStore)
        {
         
            _userNotificationManager = userNotificationManager;
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
            _entityStore = entityStore;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<ICommandResult<Comment>> SendAsync(INotificationContext<Comment> context)
        {
            
            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(WebNotifications.NewIssueComment.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<Comment>();

            // Get the topic for the reply
            var topic = await _entityStore.GetByIdAsync(context.Model.EntityId);
            if (topic == null)
            {
                return result.Failed($"No entity could be found with the Id of {context.Model.EntityId} when sending the topic follow notification '{WebNotifications.NewIssueComment.Name}'.");
            }

            // Get base Uri
            var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();

            // Build user notification
            var userNotification = new UserNotification()
            {
                NotificationName = context.Notification.Type.Name,
                UserId = context.Notification.To.Id,
                Title = topic.Title,
                Message = S["A comment has been posted within am issue your following"],
                CreatedUserId = context.Model.CreatedUserId,
                Url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
                {
                    ["area"] = "Plato.Issues",
                    ["controller"] = "Home",
                    ["action"] = "Reply",
                    ["opts.id"] = topic.Id,
                    ["opts.alias"] = topic.Alias,
                    ["opts.replyId"] = context.Model.Id
                })
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
