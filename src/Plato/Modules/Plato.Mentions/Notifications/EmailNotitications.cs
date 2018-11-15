using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.Extensions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Mentions.Notifications
{
    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification NewMention =
            new EmailNotification("NewMentionEmail", "New Mentions",
                "Show me a web notification for each new @mention.", Sender());
        
        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewMention
            };

        }

        public IEnumerable<INotificationType> GetDefaultPermissions()
        {
            throw new NotImplementedException();
        }
        
        static Func<INotificationContext, Task<ICommandResult<Notification>>> Sender()
        {
            return async context =>
            {

                // Get services
                var contextFacade = context.ServiceProvider.GetRequiredService<IContextFacade>();
                var localeStore = context.ServiceProvider.GetRequiredService<ILocaleStore>();
                var emailManager = context.ServiceProvider.GetRequiredService<IEmailManager>();

                // Build result
                var result = new CommandResult<Notification>();

                // Get email
                var culture = await contextFacade.GetCurrentCultureAsync();
                var email = await localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, "ConfirmEmail");
                if (email != null)
                {

                    // Build email confirmation link
                    var baseUrl = await contextFacade.GetBaseUrlAsync();
                    var callbackUrl = baseUrl + contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Users",
                        ["Controller"] = "Account",
                        ["Action"] = "ActivateAccount",
                        ["Code"] = context.Notification.To.ConfirmationToken
                    });

                    var body = string.Format(email.Message, context.Notification.To.DisplayName, callbackUrl);

                    var message = new MailMessage()
                    {
                        Subject = email.Subject,
                        Body = WebUtility.HtmlDecode(body),
                        IsBodyHtml = true
                    };

                    message.To.Add(context.Notification.To.Email);

                    // send email
                    var emailResult = await emailManager.SaveAsync(message);
                    return emailResult.Succeeded
                        ? result.Success()
                        : result.Failed(emailResult.Errors?.ToArray());

                }
                
                return result;

            };

        }
        
    }

}
