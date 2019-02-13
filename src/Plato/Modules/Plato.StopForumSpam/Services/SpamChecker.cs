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

            // Get configured spam level
            var level = await GetLevel();

            // We always need a level to check against
            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            // Get StopForumSpam results
            var frequencies = await _spamFrequencies.GetAsync(user);

            // Ensure we have results
            if (frequencies == null)
            {
                return false;
            }

            // If an error occurs whilst communicating with SFS simply return false
            if (!frequencies.Success)
            {
                return false;
            }
            
            // Check results against configured level 

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

        async Task<SpamLevel> GetLevel()
        {
            var settings = await _stopForumSpamSettingsStore.GetAsync();
            return DefaultSpamLevels.SpamLevels.FirstOrDefault(l => l.Tick == settings?.SpamLevel) ??
                   DefaultSpamLevels.SpamLevels.FirstOrDefault(l => l.Tick == DefaultSpamLevels.DefaultSpamLevel);
        }

    }

}
