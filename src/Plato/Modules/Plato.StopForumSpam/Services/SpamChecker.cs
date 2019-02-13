using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
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

        public async Task<bool> CheckAsync(IUser user)
        {

            // Get StopForumSpam settings
            var settings = await _stopForumSpamSettingsStore.GetAsync();

            // If we don't have any settings or an API key always return
            // false as we are unable to determine if the details are SPAM

            if (settings == null)
            {
                return false;
            }

            // We always need an API key
            if (string.IsNullOrEmpty(settings?.ApiKey))
            {
                return false;
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
                return false;
            }

            // If an error occurs simply return false
            if (!frequencies.Success)
            {
                return false;
            }

            // We have frequencies to check against our configured spam level
            // Go ahead and check frequencies against configured level 

            if (level.Frequencies.UserName.Appears)
            {
                if (frequencies.UserName.Appears)
                {
                    return true;
                }
            }

            if (level.Frequencies.Email.Appears)
            {
                if (frequencies.Email.Appears)
                {
                    return true;
                }
            }

            if (level.Frequencies.IpAddress.Appears)
            {
                if (frequencies.IpAddress.Appears)
                {
                    return true;
                }
            }
            
            return false;

        }

        private SpamLevel GetLevel(StopForumSpamSettings settings)
        {
            return DefaultSpamLevels.SpamLevels.FirstOrDefault(l => l.Tick == settings?.SpamLevel) ??
                   DefaultSpamLevels.SpamLevels.FirstOrDefault(l => l.Tick == DefaultSpamLevels.DefaultSpamLevel);
        }

    }

}
