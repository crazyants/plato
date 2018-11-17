using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Email.Models;
using Plato.Email.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Email.Services
{
    
    public class EmailManager : IEmailManager
    {

        private readonly SmtpSettings _smtpSettings;
        private readonly IEmailStore<EmailMessage> _emailStore;
        private readonly ISmtpService _smtpService;
        private readonly ILogger<EmailManager> _logger;
        private readonly IBroker _broker;

        public EmailManager(
            IEmailStore<EmailMessage> emailStore,
            ISmtpService smtpService,
            IOptions<SmtpSettings> options,
            ILogger<EmailManager> logger,
            IBroker broker)
        {
            _emailStore = emailStore;
            _smtpService = smtpService;
            _smtpSettings = options.Value;
            _logger = logger;
            _broker = broker;
        }
        
        public async Task<ICommandResult<EmailMessage>> SaveAsync(MailMessage message)
        {
            
            var result = new CommandResult<EmailMessage>();

            // Ensure we've configured required email settings
            if (_smtpSettings?.DefaultFrom == null)
            {
                return result.Failed("Email settings must be configured before an email can be sent. No default 'From' address had been specified!");
            }

            // Use application email if no from is specified
            if (message.From == null)
            {
                message.From = new MailAddress(_smtpSettings.DefaultFrom);
            }

            // Invoke EmailCreating subscriptions
            foreach (var handler in _broker.Pub<MailMessage>(this, new MessageOptions()
            {
                Key = "EmailCreating"
            }, message))
            {
                message = await handler.Invoke(new Message<MailMessage>(message, this));
            }

            // Persist the message
            var email = await _emailStore.CreateAsync(new EmailMessage(message));
            if (email != null)
            {
                // Invoke EmailCreated subscriptions
                foreach (var handler in _broker.Pub<MailMessage>(this, new MessageOptions()
                {
                    Key = "EmailCreated"
                }, message))
                {
                    message = await handler.Invoke(new Message<MailMessage>(message, this));
                }
                return result.Success(email);
            }

            return result.Failed($"An unknown error occurred whilst attempting to queue an email message");

        }

        public async Task<ICommandResult<MailMessage>> SendAsync(MailMessage message)
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

            // Invoke EmailSending subscriptions
            foreach (var handler in _broker.Pub<MailMessage>(this, new MessageOptions()
            {
                Key = "EmailSending"
            }, message))
            {
                message = await handler.Invoke(new Message<MailMessage>(message, this));
            }
            
            // Attempt to send the email
            var sendResult = await _smtpService.SendAsync(message);
            if (sendResult.Succeeded)
            {
                // Invoke EmailSent subscriptions
                foreach (var handler in _broker.Pub<MailMessage>(this, new MessageOptions()
                {
                    Key = "EmailSent"
                }, message))
                {
                    message = await handler.Invoke(new Message<MailMessage>(message, this));
                }
                return result.Success(message);
            }

            return result.Failed($"An unknown error occurred whilst attempting to send an email message");
            
        }

    }

}
