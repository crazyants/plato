using System.Threading.Tasks;
using Plato.Internal.Models.Users;

namespace Plato.StopForumSpam.Services
{
    public interface ISpamChecker
    {
        Task<bool> CheckAsync(IUser user);
    }
}
