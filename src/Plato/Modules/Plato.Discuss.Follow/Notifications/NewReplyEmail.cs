using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Discuss.Follow.NotificationTypes;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.Extensions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;


namespace Plato.Discuss.Follow.Notifications
{

    public class NewReplyEmail : INotificationProvider<Reply>
    {

        private readonly IContextFacade _contextFacade;
        private readonly ILocaleStore _localeStore;
        private readonly IEmailManager _emailManager;
        private readonly IEntityStore<Topic> _topicStore;
        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;

        public NewReplyEmail(
            IContextFacade contextFacade,
            ILocaleStore localeStore,
            IEmailManager emailManager,
            IEntityStore<Topic> topicStore,
            ICapturedRouterUrlHelper capturedRouterUrlHelper)
        {
            _contextFacade = contextFacade;
            _localeStore = localeStore;
            _emailManager = emailManager;
            _topicStore = topicStore;
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
        }

        public async Task<ICommandResult<Reply>> SendAsync(INotificationContext<Reply> context)
        {
            
            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(EmailNotifications.NewReply.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<Reply>();
            
            // Get the topic for the reply
            var topic = await _topicStore.GetByIdAsync(context.Model.EntityId);
            if (topic == null)
            {
                return result.Failed($"No entity could be found with the Id of {context.Model.EntityId} when sending the topic follow notification '{EmailNotifications.NewReply.Name}'.");
            }

            // Get email template
            const string templateId = "NewReply";
            var culture = await _contextFacade.GetCurrentCultureAsync();
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, templateId);
            if (email != null)
            {

                // Build topic url
                var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();
                var url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
                {
                    ["Area"] = "Plato.Discuss",
                    ["Controller"] = "Home",
                    ["Action"] = "Topic",
                    ["Id"] = topic.Id,
                    ["Alias"] = topic.Alias
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
