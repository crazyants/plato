using System;
using System.Net;
using System.Threading.Tasks;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.Services
{
    public interface IStopForumSpamClient
    {

        StopForumSpamClientOptions Options { get; }

        void Configure(Action<StopForumSpamClientOptions> configure);

        Task<Response> CheckEmailAddressAsync(string emailAddress);

        Task<Response> CheckUsernameAsync(string username);

        Task<Response> CheckIpAddressAsync(string ipAddress);

        Task<Response> CheckIpAddressAsync(IPAddress ipAddress);

        Task<Response> CheckAsync(string username, string emailAddress, string ipAddress);

        Task<Response> CheckAsync(string username, string emailAddress, IPAddress ipAddress);

    }

}
