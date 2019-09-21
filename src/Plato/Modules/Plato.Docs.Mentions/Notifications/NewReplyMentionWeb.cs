using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Docs.Mentions.NotificationTypes;
using Plato.Docs.Models;
using Plato.Entities.Extensions;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Docs.Mentions.Notifications
{

    public class NewReplyMentionWeb : INotificationProvider<DocComment>
    {

        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Doc> _entityStore;
        private readonly IUserNotificationsManager<UserNotification> _userNotificationManager;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public NewReplyMentionWeb(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IUserNotificationsManager<UserNotification> userNotificationManager,
            IContextFacade contextFacade,
            IEntityStore<Doc> entityStore)
        {
            _userNotificationManager = userNotificationManager;
            _contextFacade = contextFacade;
            _entityStore = entityStore;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<ICommandResult<DocComment>> SendAsync(INotificationContext<DocComment> context)
        {

            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(WebNotifications.NewMention.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // We always need a model
            if (context.Model == null)
            {
                return null;
            }

            // The reply should be visible
            if (context.Model.IsHidden())
            {
                return null;
            }

            // Get entity for reply
            var entity = await _entityStore.GetByIdAsync(context.Model.EntityId);

            // We need an entity
            if (entity == null)
            {
                return null;
            }

            // The entity should be visible
            if (entity.IsHidden())
            {
                return null;
            }
            
            // Create result
            var result = new CommandResult<DocComment>();
            
            // Build user notification
            var userNotification = new UserNotification()
            {
                NotificationName = context.Notification.Type.Name,
                UserId = context.Notification.To.Id,
                Title = S["New Mention"].Value,
                Message = S["You've been mentioned by "].Value + context.Model.CreatedBy.DisplayName,
                CreatedUserId = context.Model.CreatedUserId,
                Url = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["area"] = "Plato.Docs",
                    ["controller"] = "Home",
                    ["action"] = "Reply",
                    ["opts.id"] = entity.Id,
                    ["opts.alias"] = entity.Alias,
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
