using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.Extensions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Mentions.NotificationTypes;

namespace Plato.Mentions.Notifications
{

    //public class NewMentionEmail : INotificationProvider
    //{

    //    private readonly IContextFacade _contextFacade;
    //    private readonly ILocaleStore _localeStore;
    //    private readonly IEmailManager _emailManager;

    //    public NewMentionEmail(
    //        IContextFacade contextFacade,
    //        ILocaleStore localeStore,
    //        IEmailManager emailManager)
    //    {
    //        _contextFacade = contextFacade;
    //        _localeStore = localeStore;
    //        _emailManager = emailManager;
    //    }

    //    public async Task<ICommandResultBase> SendAsync<T>(INotificationContext<T> context) where T : class
    //    {

    //        var result = new CommandResultBase();

    //        if (context.Notification.Type.Id !=
    //            EmailNotifications.NewMention.Id)
    //        {
    //            return result.Success();
    //        }

    //        // Get email
    //        var model = context.Model;
        
    //        var culture = await _contextFacade.GetCurrentCultureAsync();
    //        var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, "NewMention");
    //        if (email != null)
    //        {

    //            // Build email confirmation link
    //            var baseUrl = await _contextFacade.GetBaseUrlAsync();
    //            var callbackUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
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
    //            var emailResult = await _emailManager.SaveAsync(message);
    //            return emailResult.Succeeded
    //                ? result.Success()
    //                : result.Failed(emailResult.Errors?.ToArray());

    //        }

    //        return result;

    //    }

    //}


}
