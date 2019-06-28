using System;
using System.Net.Mail;

namespace Plato.Internal.Emails.Abstractions.Extensions
{
    public static class EmailMessageExtensions
    {

        public static MailMessage ToMailMessage(this EmailMessage message)
        {

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var output = new MailMessage();

            if (!string.IsNullOrEmpty(message.To))
            {
                output.To.Add(message.To);
            }

            if (!string.IsNullOrEmpty(message.Cc))
            {
                output.CC.Add(message.To);
            }

            if (!string.IsNullOrEmpty(message.Bcc))
            {
                output.Bcc.Add(message.To);
            }

            if (!string.IsNullOrEmpty(message.From))
            {
                output.From = new MailAddress(message.From);
            }
            
            output.Subject = message.Subject;
            output.Body = message.Body;
            output.Priority = message.Priority;
            
            return output;

        }

    }

}
