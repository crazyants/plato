using System.Net.Mail;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.Extensions
{

    public static class LocaleEmailExtensions
    {

        public static MailMessage BuildMailMessage(this LocaleEmail email)
        {

            var message = new MailMessage();

            if (!string.IsNullOrEmpty(email.To))
            {
                message.To.Add(new MailAddress(email.To));
            }

            if (!string.IsNullOrEmpty(email.Cc))
            {
                message.CC.Add(new MailAddress(email.Cc));
            }

            if (!string.IsNullOrEmpty(email.Bcc))
            {
                message.Bcc.Add(new MailAddress(email.Bcc));
            }

            if (!string.IsNullOrEmpty(email.From))
            {
                message.From = new MailAddress(email.From);
            }

            if (!string.IsNullOrEmpty(email.Subject))
            {
                message.Subject = email.Subject;
            }

            if (!string.IsNullOrEmpty(email.Message))
            {
                message.Body = email.Message;
            }
            
            return message;

        }

    }

}
