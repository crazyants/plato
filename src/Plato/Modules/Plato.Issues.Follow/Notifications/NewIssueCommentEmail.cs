using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Issues.Follow.NotificationTypes;
using Plato.Issues.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.Extensions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Tasks.Abstractions;


namespace Plato.Issues.Follow.Notifications
{

    public class NewIssueCommentEmail : INotificationProvider<Comment>
    {

        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;
        private readonly IEntityStore<Issue> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly IEmailManager _emailManager;
        private readonly ILocaleStore _localeStore;

        public NewIssueCommentEmail(
            ICapturedRouterUrlHelper capturedRouterUrlHelper,
            IEntityStore<Issue> entityStore,
            IContextFacade contextFacade,
            IEmailManager emailManager,
            ILocaleStore localeStore)
        {
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
            _contextFacade = contextFacade;
            _emailManager = emailManager;
            _localeStore = localeStore;
            _entityStore = entityStore;
        }

        public async Task<ICommandResult<Comment>> SendAsync(INotificationContext<Comment> context)
        {
            
            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(EmailNotifications.NewIssueComment.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<Comment>();
            
            // Get the topic for the reply
            var topic = await _entityStore.GetByIdAsync(context.Model.EntityId);
            if (topic == null)
            {
                return result.Failed($"No entity could be found with the Id of {context.Model.EntityId} when sending the topic follow notification '{EmailNotifications.NewIssueComment.Name}'.");
            }

            // Get email template
            const string templateId = "NewIssueComment";
            var culture = await _contextFacade.GetCurrentCultureAsync();
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, templateId);
            if (email != null)
            {

                // Build topic url
                var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();
                var url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
                {
                    ["area"] = "Plato.Issues",
                    ["controller"] = "Home",
                    ["action"] = "Reply",
                    ["opts.id"] = topic.Id,
                    ["opts.alias"] = topic.Alias,
                    ["opts.replyId"] = context.Model.Id
                });
                
                // Build message from template
                var message = email.BuildMailMessage();
                message.Body = string.Format(
                    email.Message,
                    context.Notification.To.DisplayName,
                    topic.Title,
                    baseUri + url);
                message.IsBodyHtml = true;
                message.To.Add(new MailAddress(context.Notification.To.Email));

                // Send message
                var emailResult = await _emailManager.SaveAsync(message);
                if (emailResult.Succeeded)
                {
                    return result.Success(context.Model);
                }

                return result.Failed(emailResult.Errors?.ToArray());

            }

            return result.Failed($"No email template with the Id '{templateId}' exists within the 'locales/{culture}/emails.json' file!");

        }

    }
    
}
