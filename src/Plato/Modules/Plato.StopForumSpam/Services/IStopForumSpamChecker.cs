using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.Services
{

    public interface IStopForumSpamChecker
    {

        void Configure(Action<StopForumSpamClientOptions> configure);
        
        StopForumSpamClientOptions Options { get; set; }

        Task<SpamFrequencies> CheckAsync(string userName, string email, string ipAddress);

        Task<SpamFrequencies> CheckAsync(IUser user);

    }
    
}
