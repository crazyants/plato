using System.Net.Mail;
using System.Threading.Tasks;
using Plato.Email.Models;
using Plato.Internal.Abstractions;

namespace Plato.Email.Services
{
    public interface IEmailManager
    {
        Task<IActivityResult<EmailMessage>> SaveAsync(MailMessage message);

        Task<IActivityResult<MailMessage>> SendAsync(MailMessage message);

    }

}
