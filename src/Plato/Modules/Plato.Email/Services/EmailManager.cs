using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Plato.Email.Models;
using Plato.Email.Stores;

namespace Plato.Email.Services
{

    public interface IEmailManager
    {
        Task QueueAsync(MailMessage message);

        Task SendAsync();

    }

    public class EmailManager : IEmailManager
    {

        private readonly IEmailStore<EmailMessage> _emailStore;

        public EmailManager(IEmailStore<EmailMessage> emailStore)
        {
            _emailStore = emailStore;
        }

        public async Task QueueAsync(MailMessage message)
        {
            await _emailStore.CreateAsync(new EmailMessage(message));
        }

        public Task SendAsync()
        {
            throw new NotImplementedException();
        }
    }
}
