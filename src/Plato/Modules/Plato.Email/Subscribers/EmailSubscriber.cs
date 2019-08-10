using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Email.Subscribers
{

    public class EmailSubscriber : IBrokerSubscriber
    {

        private readonly IOptions<SmtpSettings> _smtpSettings;
        private readonly IContextFacade _contextFacade;
        private readonly IBroker _broker;
        private readonly ICapturedRouterUrlHelper _urlHelper;


        public EmailSubscriber(
            IBroker broker,
            IContextFacade contextFacade,
            IOptions<SmtpSettings> smtpSettings,
            ICapturedRouterUrlHelper urlHelper)
        {
            _broker = broker;
            _contextFacade = contextFacade;
            _smtpSettings = smtpSettings;
            _urlHelper = urlHelper;
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

                mailMessage.Subject = mailMessage.Subject.Replace(replacement.Key, value);
                mailMessage.Body = mailMessage.Body.Replace(replacement.Key, value);

            }

            return mailMessage;

        }

        async Task<IDictionary<string, string>> GetReplacements()
        {

            var settings = await _contextFacade.GetSiteSettingsAsync();
            var baseUrl = await _urlHelper.GetBaseUrlAsync();

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
