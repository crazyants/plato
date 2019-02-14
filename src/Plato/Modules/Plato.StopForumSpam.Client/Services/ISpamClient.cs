using System;
using System.Net;
using System.Threading.Tasks;
using Plato.StopForumSpam.Client.Models;

namespace Plato.StopForumSpam.Client.Services
{
    public interface ISpamClient
    {

        ClientOptions Options { get; }

        void Configure(Action<ClientOptions> configure);

        Task<Response> CheckEmailAddressAsync(string emailAddress);

        Task<Response> CheckUsernameAsync(string username);

        Task<Response> CheckIpAddressAsync(string ipAddress);

        Task<Response> CheckIpAddressAsync(IPAddress ipAddress);

        Task<Response> CheckAsync(string username, string emailAddress, string ipAddress);

        Task<Response> CheckAsync(string username, string emailAddress, IPAddress ipAddress);

    }

}
