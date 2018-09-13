using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Email.Models;
using Plato.Email.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;

namespace Plato.Email.Services
{
    
    public class EmailManager : IEmailManager
    {

        private readonly SmtpSettings _smtpSettings;
        private readonly IEmailStore<EmailMessage> _emailStore;
        private readonly ISmtpService _smtpService;
        private readonly ILogger<EmailManager> _logger;

        public EmailManager(
            IEmailStore<EmailMessage> emailStore,
            ISmtpService smtpService,
            IOptions<SmtpSettings> options,
            ILogger<EmailManager> logger)
        {
            _emailStore = emailStore;
            _smtpService = smtpService;
            _logger = logger;
            _smtpSettings = options.Value;
        }
        
        public async Task<IActivityResult<EmailMessage>> SaveAsync(MailMessage message)
        {
            
            var result = new ActivityResult<EmailMessage>();

            // Ensure we've configured required email settings
            if (_smtpSettings?.DefaultFrom == null)
            {
                return result.Failed("Email settings must be configured before an email can be sent.");
            }

            // Use application email if no from is specified
            if (message.From == null)
            {
                message.From = new MailAddress(_smtpSettings.DefaultFrom);
            }

            // Persist the message
            var email = await _emailStore.CreateAsync(new EmailMessage(message));
            if (email != null)
            {
                return result.Success(email);
            }

            return result.Failed($"An unknown error occurred whilst attempting to queue an email message");

        }

        public async Task<IActivityResult<MailMessage>> SendAsync(MailMessage message)
        {
            var result = new SmtpResult();

            // Ensure we've configured required email settings
            if (_smtpSettings?.DefaultFrom == null)
            {
                return result.Failed("Email settings must be configured before an email can be sent.");
            }

            // Use application email if no from is specified
            if (message.From == null)
            {
                message.From = new MailAddress(_smtpSettings.DefaultFrom);
            }
            
            // Attempt to send the email
            return await _smtpService.SendAsync(message);

        }

    }
}
