using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Client.Models;
using Plato.StopForumSpam.Client.Services;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Stores;

namespace Plato.StopForumSpam.Services
{

    public class SpamChecker : ISpamChecker
    {
        private readonly ISpamFrequencies _spamFrequencies;
        private readonly IStopForumSpamSettingsStore<StopForumSpamSettings> _stopForumSpamSettingsStore;

        public SpamChecker(
            ISpamFrequencies spamFrequencies,
            IStopForumSpamSettingsStore<StopForumSpamSettings> stopForumSpamSettingsStore)
        {
            _spamFrequencies = spamFrequencies;
            _stopForumSpamSettingsStore = stopForumSpamSettingsStore;
        }
        
        public async Task<ISpamCheckerResult> CheckAsync(IUser user)
        {
            
            // Get StopForumSpam settings
            var settings = await _stopForumSpamSettingsStore.GetAsync();
            
            // Identify which flags to check
            var level = settings != null
                ? SpamLevelDefaults.SpamLevels.FirstOrDefault(l => l.Id == settings.SpamLevelId)
                : SpamLevelDefaults.SpamLevels.FirstOrDefault(l => l.Id == SpamLevelDefaults.SpamLevelId);
            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            // Return result
            return await CheckAsync(user, settings?.ApiKey ?? string.Empty, level.Flags);

        }
        
        public async Task<ISpamCheckerResult> CheckAsync(IUser user, RequestType flags)
        {
            
            // Get StopForumSpam settings
            var settings = await _stopForumSpamSettingsStore.GetAsync();
            
            // Return result
            return await CheckAsync(user, settings?.ApiKey ?? string.Empty, flags);

        }

        #region "Private Methods"
        
        async Task<ISpamCheckerResult> CheckAsync(
            IUser user,
            string apiKey,
            RequestType checkAgainst)
        {

            // The result we'll return
            var result = new SpamCheckerResult();

            // We always need an API key to perform checks
            if (string.IsNullOrEmpty(apiKey))
            {
                return result.Success();
            }
           
            // Configure frequency checker
            _spamFrequencies.Configure(o => { o.ApiKey = apiKey; });

            // Get frequencies
            var frequencies = await _spamFrequencies.GetAsync(user);

            // Ensure we have frequencies
            if (frequencies == null)
            {
                return result.Success();
            }

            // If an error occurs whilst obtaining frequencies simply return success
            if (!frequencies.Success)
            {
                return result.Success();
            }
            
            // Check flags
            var errors = new List<RequestType>();
            switch (checkAgainst)
            {
                case RequestType.Username | RequestType.EmailAddress | RequestType.IpAddress:

                    if (frequencies.UserName.Appears && frequencies.Email.Appears & frequencies.IpAddress.Appears)
                    {
                        errors.Add(RequestType.Username);
                        errors.Add(RequestType.EmailAddress);
                        errors.Add(RequestType.IpAddress);
                    }

                    break;

                case RequestType.EmailAddress | RequestType.IpAddress:

                    if (frequencies.Email.Appears & frequencies.IpAddress.Appears)
                    {
                        errors.Add(RequestType.EmailAddress);
                        errors.Add(RequestType.IpAddress);
                    }

                    break;

                case RequestType.IpAddress:

                    if (frequencies.IpAddress.Appears)
                    {
                        errors.Add(RequestType.IpAddress);
                    }

                    break;

            }

            return errors.Count > 0
                ? result.Fail(errors)
                : result.Success();

        }

        #endregion

    }

}
