using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.Services
{
    
    public class StopForumSpamChecker : IStopForumSpamChecker
    {

        private readonly IStopForumSpamClient _stopForumSpamClient;

        public StopForumSpamChecker(
            IStopForumSpamClient stopForumSpamClient)
        {
            _stopForumSpamClient = stopForumSpamClient;
        }

        public StopForumSpamClientOptions Options { get; private set; }

        public void Configure(Action<StopForumSpamClientOptions> configure)
        {
            _stopForumSpamClient.Configure(configure);
            this.Options = _stopForumSpamClient.Options;
        }

        public async Task<SpamFrequencies> CheckAsync(string userName, string email, string ipAddress)
        {
            return await CheckAsync(new User()
            {
                UserName = userName,
                Email = email,
                IpV4Address = ipAddress
            });
        }

        public async Task<SpamFrequencies> CheckAsync(IUser user)
        {

            // We need options
            if (this.Options == null)
            {
                throw new ArgumentNullException(nameof(this.Options));
            }

            // Make request & get response
            var spamResponse = await _stopForumSpamClient.CheckAsync(
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

            return new SpamFrequencies()
            {
                UserName = new SpamFrequency(usernameFrequency),
                Email = new SpamFrequency(emailFrequency),
                IpAddress = new SpamFrequency(ipFrequency),
                Success = success
            };
            
        }

    }

}
