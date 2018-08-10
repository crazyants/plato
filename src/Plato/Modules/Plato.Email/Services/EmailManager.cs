using System.Net.Mail;
using System.Threading.Tasks;
using Plato.Email.Models;
using Plato.Email.Stores;
using Plato.Internal.Abstractions;

namespace Plato.Email.Services
{
    
    public class EmailManager : IEmailManager
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly IEmailStore<EmailMessage> _emailStore;
        private readonly ISmtpService _smtpService;

        public EmailManager(
            IEmailStore<EmailMessage> emailStore,
            ISmtpService smtpService,
            SmtpSettings smtpSettings)
        {
            _emailStore = emailStore;
            _smtpService = smtpService;
            _smtpSettings = smtpSettings;
        }
        
        public async Task<IActivityResult<EmailMessage>> SaveAsync(MailMessage message)
        {
            
            var result = new ActivityResult<EmailMessage>();

            if (_smtpSettings?.DefaultFrom == null)
            {
                return result.Failed("SMTP settings must be configured before an email can be saved to the queue.");
            }

            if (message.From == null)
            {
                message.From = new MailAddress(_smtpSettings.DefaultFrom);
            }

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
            if (_smtpSettings?.DefaultFrom == null)
            {
                return result.Failed("SMTP settings must be configured before an email can be sent.");
            }

            if (message.From == null)
            {
                message.From = new MailAddress(_smtpSettings.DefaultFrom);
            }
            
            return await _smtpService.SendAsync(message);

        }

    }
}
