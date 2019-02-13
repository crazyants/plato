using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Client.Models;
using Plato.StopForumSpam.Client.Services;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Stores;
using Plato.StopForumSpam.ViewModels;

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

            // The result we'll return
            var result = new SpamCheckerResult();
            
            // If we don't have any settings or an API key always return
            // false as we are unable to determine if the details are SPAM

            if (settings == null)
            {
                return result.Success();
            }

            // We always need an API key
            if (string.IsNullOrEmpty(settings?.ApiKey))
            {
                return result.Success();
            }

            // Get configured spam level
            var level = GetLevel(settings);

            // We always need a level to check against
            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            // Configure frequency checker
            _spamFrequencies.Configure(o =>
            {
                o.ApiKey = settings?.ApiKey ?? "";
            });

            // Get frequencies
            var frequencies = await _spamFrequencies.GetAsync(user);

            // Ensure we have frequencies
            if (frequencies == null)
            {
                return result.Success();
            }

            // If an error occurs simply return false
            if (!frequencies.Success)
            {
                return result.Success();
            }

            // We have frequencies to check against our configured spam level
            // Go ahead and check frequencies against configured level 
            
            if (level.Frequencies.UserName.Appears)
            {
                if (frequencies.UserName.Appears)
                {
                    result.Error(RequestType.Username);
                }
            }

            if (level.Frequencies.Email.Appears)
            {
                if (frequencies.Email.Appears)
                {
                    result.Error(RequestType.EmailAddress);
                }
            }

            if (level.Frequencies.IpAddress.Appears)
            {
                if (frequencies.IpAddress.Appears)
                {
                    result.Error(RequestType.IpAddress);
                }
            }

            return result;

        }

        private SpamLevel GetLevel(StopForumSpamSettings settings)
        {
            return DefaultSpamLevels.SpamLevels.FirstOrDefault(l => l.Tick == settings?.SpamLevel) ??
                   DefaultSpamLevels.SpamLevels.FirstOrDefault(l => l.Tick == DefaultSpamLevels.DefaultSpamLevel);
        }

    }

}
