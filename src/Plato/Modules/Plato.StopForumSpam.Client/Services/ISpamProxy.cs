using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Client.Models;

namespace Plato.StopForumSpam.Client.Services
{

    public interface ISpamProxy
    {

        ClientOptions Options { get; }

        void Configure(Action<ClientOptions> configure);
        
        Task<IProxyResults> GetAsync(string userName, string email, string ipAddress);

        Task<IProxyResults> GetAsync(IUser user);

    }
    
}
