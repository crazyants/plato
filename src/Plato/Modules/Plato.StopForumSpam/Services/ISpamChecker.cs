using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Client.Models;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.Services
{
    public interface ISpamChecker
    {

        Task<ISpamCheckerResult> CheckAsync(IUser user);

        Task<ISpamCheckerResult> CheckAsync(IUser user, RequestType flags);

    }

}
