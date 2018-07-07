using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Plato.Email.Models;
using Plato.Email.Stores;
using Plato.Internal.Abstractions;

namespace Plato.Email.Services
{

    public interface IEmailManager
    {
        Task QueueAsync(MailMessage message);

        Task<IActivityResult<MailMessage>> SendAsync();

    }

    public class EmailManager : IEmailManager
    {

        private readonly IEmailStore<EmailMessage> _emailStore;
        private readonly SmtpSettings _smtpSettings;

        public EmailManager(
            IEmailStore<EmailMessage> emailStore,
            SmtpSettings smtpSettings)
        {
            _emailStore = emailStore;
            _smtpSettings = smtpSettings;
        }

        public async Task QueueAsync(MailMessage message)
        {
            await _emailStore.CreateAsync(new EmailMessage(message));
        }

        public Task<IActivityResult<MailMessage>> SendAsync()
        {
        
            throw new NotImplementedException();
        }
    }
}
