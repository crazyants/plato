using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Email.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Email.Subscribers
{

    public class EmailSubscriber : IBrokerSubscriber
    {

        private readonly IOptions<SmtpSettings> _smtpSettings;
        private readonly IContextFacade _contextFacade;
        private readonly ILogger<EmailSubscriber> _logger;
        private readonly IBroker _broker;

        public EmailSubscriber(
            ILogger<EmailSubscriber> logger,
            IBroker broker,
            IContextFacade contextFacade,
            IOptions<SmtpSettings> smtpSettings)
        {
            _broker = broker;
            _contextFacade = contextFacade;
            _smtpSettings = smtpSettings;
            _logger = logger;
        }

        #region "Implementation"

        public void Subscribe()
        {
            // EmailCreating
            _broker.Sub<MailMessage>(new MessageOptions()
            {
                Key = "EmailCreating"
            }, async message => await EmailCreating(message.What));

        }

        public void Unsubscribe()
        {
            // EmailCreating
            _broker.Unsub<MailMessage>(new MessageOptions()
            {
                Key = "EmailCreating"
            }, async message => await EmailCreating(message.What));
            
        }

        #endregion

        #region "Private Methods"

        async Task<MailMessage> EmailCreating(MailMessage mailMessage)
        {

            foreach (var replacement in await GetReplacements())
            {

                var value = !string.IsNullOrEmpty(replacement.Value)
                    ? replacement.Value
                    : string.Empty;

                // To
                for (var i = 0; i < mailMessage.To.Count; i++)
                {
                    mailMessage.To[i] = new MailAddress(
                        mailMessage.To[i].Address.Replace(replacement.Key, value),
                        mailMessage.To[i].DisplayName.Replace(replacement.Key, value));
                }

                // CC
                for (var i = 0; i < mailMessage.CC.Count; i++)
                {
                    mailMessage.CC[i] = new MailAddress(
                        mailMessage.CC[i].Address.Replace(replacement.Key, value),
                        mailMessage.CC[i].DisplayName.Replace(replacement.Key, value));
                }

                // BCC
                for (var i = 0; i < mailMessage.Bcc.Count; i++)
                {
                    mailMessage.Bcc[i] = new MailAddress(
                        mailMessage.Bcc[i].Address.Replace(replacement.Key, value),
                        mailMessage.Bcc[i].DisplayName.Replace(replacement.Key, value));
                }
                
                // From
                mailMessage.From = new MailAddress(
                    mailMessage.From.Address.Replace(replacement.Key, value),
                    mailMessage.From.DisplayName.Replace(replacement.Key, value));

                // Subject
                mailMessage.Subject = mailMessage.Subject.Replace(replacement.Key, value);

                // Body
                mailMessage.Body = mailMessage.Body.Replace(replacement.Key, value);

            }

            return mailMessage;

        }

        async Task<IDictionary<string, string>> GetReplacements()
        {

            var settings = await _contextFacade.GetSiteSettingsAsync();
            var baseUrl = await _contextFacade.GetBaseUrlAsync();

            return new Dictionary<string, string>
            {
                {"[SiteName]", settings.SiteName},
                {"[SiteEmail]", _smtpSettings.Value.DefaultFrom},
                {"[SiteUrl]", baseUrl}
            };

        }

        #endregion

    }

}
