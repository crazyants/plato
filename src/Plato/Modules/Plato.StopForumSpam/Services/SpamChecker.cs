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
        private readonly ISpamProxy _spamProxy;
        private readonly ISpamSettingsStore<SpamSettings> _spamSettingsStore;

        public SpamChecker(
            ISpamProxy spamProxy,
            ISpamSettingsStore<SpamSettings> spamSettingsStore)
        {
            _spamProxy = spamProxy;
            _spamSettingsStore = spamSettingsStore;
        }

        #region "Implementation"

        public async Task<ISpamCheckerResult> CheckAsync(IUser user)
        {
            
            // Get StopForumSpam settings
            var settings = await _spamSettingsStore.GetAsync();
            
            // Identify which flags to check
            var level = settings != null
                ? SpamLevelDefaults.SpamLevels.FirstOrDefault(l => l.Id == settings.SpamLevelId)
                : SpamLevelDefaults.SpamLevels.FirstOrDefault(l => l.Id == SpamLevelDefaults.SpamLevelId);
            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            // Return result
            return await CheckInternalAsync(user, settings?.ApiKey ?? string.Empty, level.Flags);

        }
        
        public async Task<ISpamCheckerResult> CheckAsync(IUser user, RequestType flags)
        {
            
            // Get settings
            var settings = await _spamSettingsStore.GetAsync();
            
            // Return results
            return await CheckInternalAsync(user, settings?.ApiKey ?? string.Empty, flags);

        }

        #endregion

        #region "Private Methods"

        async Task<ISpamCheckerResult> CheckInternalAsync(IUser user, string apiKey, RequestType flags)
        {

            // The result we'll return
            var result = new SpamCheckerResult();

            // We always need an API key to perform checks
            if (string.IsNullOrEmpty(apiKey))
            {
                return result.Success();
            }
           
            // Configure frequency checker
            _spamProxy.Configure(o => { o.ApiKey = apiKey; });

            // Get frequencies
            var frequencies = await _spamProxy.GetAsync(user);

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
            
            // Check configured flags for configured level and compile errors
            var errors = new List<RequestType>();
            switch (flags)
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
                ? result.Fail(errors, frequencies)
                : result.Success(frequencies);

        }

        #endregion

    }

}
