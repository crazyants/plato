using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.Services
{

    public interface IStopForumSpamChecker
    {

        StopForumSpamClientOptions Options { get; }

        void Configure(Action<StopForumSpamClientOptions> configure);
        
        Task<SpamFrequencies> CheckAsync(string userName, string email, string ipAddress);

        Task<SpamFrequencies> CheckAsync(IUser user);

    }
    
}
