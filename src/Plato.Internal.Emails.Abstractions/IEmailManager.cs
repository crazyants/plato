using System.Net.Mail;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Emails.Abstractions
{ 
    public interface IEmailManager
    {
        Task<IActivityResult<EmailMessage>> SaveAsync(MailMessage message);

        Task<IActivityResult<MailMessage>> SendAsync(MailMessage message);

    }

}
