using System.Net.Mail;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Emails.Abstractions
{ 
    public interface IEmailManager
    {
        Task<ICommandResult<EmailMessage>> SaveAsync(MailMessage message);

        Task<ICommandResult<MailMessage>> SendAsync(MailMessage message);

    }

}
