using System;
using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Mentions.NotificationTypes
{
    
    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification NewMention =
            new EmailNotification("NewMentionEmail", "New Mentions",
                "Send me an email notification for each new @mention.");
        
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
        
        //static Func<INotificationContext, Task<ICommandResultBase>> Sender()
        //{
        //    return async context =>
        //    {

        //        // Get services
        //        var contextFacade = context.ServiceProvider.GetRequiredService<IContextFacade>();
        //        var localeStore = context.ServiceProvider.GetRequiredService<ILocaleStore>();
        //        var emailManager = context.ServiceProvider.GetRequiredService<IEmailManager>();

        //        // Build result
        //        var result = new CommandResultBase();

        //        // Get email
        //        var culture = await contextFacade.GetCurrentCultureAsync();
        //        var email = await localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, "ConfirmEmail");
        //        if (email != null)
        //        {

        //            // Build email confirmation link
        //            var baseUrl = await contextFacade.GetBaseUrlAsync();
        //            var callbackUrl = baseUrl + contextFacade.GetRouteUrl(new RouteValueDictionary()
        //            {
        //                ["Area"] = "Plato.Users",
        //                ["Controller"] = "Account",
        //                ["Action"] = "ActivateAccount",
        //                ["Code"] = context.Notification.To.ConfirmationToken
        //            });

        //            var body = string.Format(email.Message, context.Notification.To.DisplayName, callbackUrl);

        //            var message = new MailMessage()
        //            {
        //                Subject = email.Subject,
        //                Body = WebUtility.HtmlDecode(body),
        //                IsBodyHtml = true
        //            };

        //            message.To.Add(context.Notification.To.Email);

        //            // send email
        //            var emailResult = await emailManager.SaveAsync(message);
        //            return emailResult.Succeeded
        //                ? result.Success()
        //                : result.Failed(emailResult.Errors?.ToArray());

        //        }
                
        //        return result;

        //    };

        //}
        
    }

}
