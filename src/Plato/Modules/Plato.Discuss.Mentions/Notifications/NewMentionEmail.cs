using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Discuss.Models;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.Extensions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Mentions.NotificationTypes;

namespace Plato.Discuss.Mentions.Notifications
{

    public class NewMentionEmail : INotificationProvider<Topic>
    {

        private readonly IContextFacade _contextFacade;
        private readonly ILocaleStore _localeStore;
        private readonly IEmailManager _emailManager;

        public NewMentionEmail(
            IContextFacade contextFacade,
            ILocaleStore localeStore,
            IEmailManager emailManager)
        {
            _contextFacade = contextFacade;
            _localeStore = localeStore;
            _emailManager = emailManager;
        }

        public async Task<ICommandResult<Topic>> SendAsync(INotificationContext<Topic> context) 
        {
            
            if (context.Notification.Type.Id != EmailNotifications.NewMention.Id)
            {
                return null;
            }

            // Create result
            var result = new CommandResult<Topic>();

            // Get email template
            var model = context.Model;
            var culture = await _contextFacade.GetCurrentCultureAsync();
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, "NewMention");
            if (email != null)
            {

                // Parse email template
                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                var callbackUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["Area"] = "Plato.Discuss",
                    ["Controller"] = "Home",
                    ["Action"] = "Topic",
                    ["Id"] = model.Id,
                    ["Alias"] = model.Alias
                });

                var body = string.Format(email.Message, context.Notification.To.DisplayName, callbackUrl);

                var message = new MailMessage()
                {
                    Subject = email.Subject,
                    Body = WebUtility.HtmlDecode(body),
                    IsBodyHtml = true
                };

                message.To.Add(context.Notification.To.Email);

                // Send email
                var emailResult = await _emailManager.SaveAsync(message);
                return emailResult.Succeeded
                    ? result.Success()
                    : result.Failed(emailResult.Errors?.ToArray());

            }

            return result;

        }

    }


}
