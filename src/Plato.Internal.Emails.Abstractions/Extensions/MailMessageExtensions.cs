using System;
using System.Net.Mail;

namespace Plato.Internal.Emails.Abstractions.Extensions
{
    public static class MailMessageExtensions
    {

        public static EmailMessage ToEmailMessage(this MailMessage message)
        {

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var output = new EmailMessage();

            if (message.To.Count > 0)
            {
                output.To = message.To[0].Address;
            }

            if (message.CC.Count > 0)
            {
                output.Cc = message.CC[0].Address;
            }

            if (message.Bcc.Count > 0)
            {
                output.Bcc = message.Bcc[0].Address;
            }

            if (message.From != null)
            {
                output.From = message.From.Address;
            }

            output.Subject = message.Subject;
            output.Body = message.Body;
            output.Priority = message.Priority;
      
            return output;

        }

    }

}
