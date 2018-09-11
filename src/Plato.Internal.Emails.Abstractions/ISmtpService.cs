using System.Net.Mail;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Email.Services
{
    public interface ISmtpService
    {
        Task<IActivityResult<MailMessage>> SendAsync(MailMessage message);

    }
    
}
