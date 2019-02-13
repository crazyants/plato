using System;
using System.Collections.Generic;
using System.Text;
using Plato.StopForumSpam.Client.Models;
using Plato.StopForumSpam.ViewModels;

namespace Plato.StopForumSpam.Models
{
    public static class DefaultSpamLevels
    {

        public static int DefaultSpamLevel = 0;

        public static IEnumerable<SpamLevel> SpamLevels = new List<SpamLevel>()
        {
            new SpamLevel()
            {

                Name = "Check against the users username, email address & IP address",
                Description =
                    "All details must appear within the StopForumSpam database for the user to be flagged as SPAM. Some SPAM may get through but false positives will be reduced.",
                Frequencies = new SpamFrequencies()
                {
                    UserName = new SpamFrequency(1),
                    Email = new SpamFrequency(1),
                    IpAddress = new SpamFrequency(1)
                }
            },
            new SpamLevel()
            {

                Name = "Check against the users email address and IP address",
                Description =
                    "The users email address & IP address must appear within the StopForumSpam database for the user to be flagged as SPAM. Usernames will be ignored. Some SPAM may get through but false positives based on username will be reduced.",
                Tick = 50,
                Frequencies = new SpamFrequencies()
                {
                    Email = new SpamFrequency(1),
                    IpAddress = new SpamFrequency(1)
                }
            },
            new SpamLevel()
            {
                Name = "Check against the users IP address only",
                Description =
                    "If the users IP address appears within the StopForumSpam database the user will be flagged as SPAM. Catches the most SPAM but can lead to more false positives.",
                Tick = 100,
                Frequencies = new SpamFrequencies()
                {
                    IpAddress = new SpamFrequency(1)
                }
            }
        };
    }

}
