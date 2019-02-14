using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Client.Models;

namespace Plato.StopForumSpam.Client.Services
{
    
    public class SpamProxy : ISpamProxy
    {

        private readonly ISpamClient _spamClient;

        public SpamProxy(
            ISpamClient spamClient)
        {
            _spamClient = spamClient;
        }

        public ClientOptions Options { get; private set; }

        public void Configure(Action<ClientOptions> configure)
        {
            _spamClient.Configure(configure);
            this.Options = _spamClient.Options;
        }

        public async Task<IProxyResults> GetAsync(string userName, string email, string ipAddress)
        {
            return await GetAsync(new User()
            {
                UserName = userName,
                Email = email,
                IpV4Address = ipAddress
            });
        }

        public async Task<IProxyResults> GetAsync(IUser user)
        {

            // We need options
            if (this.Options == null)
            {
                throw new ArgumentNullException(nameof(this.Options));
            }

            // Make request & get response
            var spamResponse = await _spamClient.CheckAsync(
                user.UserName,
                user.Email,
                user.IpV4Address);
            
            // Build frequency response
            var usernameFrequency = 0;
            var emailFrequency = 0;
            var ipFrequency = 0;
            var success = false;

            if (spamResponse.ResponseParts != null)
            {
                success = true;
                foreach (var part in spamResponse.ResponseParts)
                {
                    switch (part.Type)
                    {
                        case RequestType.Username:
                        {
                            usernameFrequency += part.Frequency;
                            break;
                        }
                        case RequestType.EmailAddress:
                        {
                            emailFrequency += part.Frequency;
                            break;
                        }
                        case RequestType.IpAddress:
                        {
                            ipFrequency += part.Frequency;
                            break;
                        }
                    }

                }
            }

            return new Models.ProxyResults()
            {
                UserName = new ProxyResult(usernameFrequency),
                Email = new ProxyResult(emailFrequency),
                IpAddress = new ProxyResult(ipFrequency),
                Success = success
            };
            
        }

    }

}
