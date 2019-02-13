using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Client.Models;

namespace Plato.StopForumSpam.Client.Services
{

    public interface ISpamFrequencies
    {

        StopForumSpamClientOptions Options { get; }

        void Configure(Action<StopForumSpamClientOptions> configure);
        
        Task<Models.SpamFrequencies> GetAsync(string userName, string email, string ipAddress);

        Task<Models.SpamFrequencies> GetAsync(IUser user);

    }
    
}
