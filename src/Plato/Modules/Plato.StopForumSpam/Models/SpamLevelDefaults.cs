using System.Collections.Generic;
using Plato.StopForumSpam.Client.Models;

namespace Plato.StopForumSpam.Models
{
    public static class SpamLevelDefaults
    {

        // The default level to apply 
        public const int SpamLevelId = 50;

        public static IEnumerable<ISpamLevel> SpamLevels { get; } = new List<SpamLevel>()
        {
            new SpamLevel
            {
                Name = "Check against the users username, email address & IP address",
                Description = "All details must appear within the StopForumSpam database for the user to be flagged as SPAM. Some SPAM may get through but false positives will be reduced.",
                Flags = RequestType.Username | RequestType.EmailAddress | RequestType.IpAddress
            },
            new SpamLevel
            {
                Id = 50,
                Name = "Check against the users email address & IP address",
                Description = "The users email address & IP address must appear within the StopForumSpam database for the user to be flagged as SPAM. Usernames will be ignored. Some SPAM may get through but false positives based on common usernames will be reduced.",
                Flags = RequestType.EmailAddress | RequestType.IpAddress
            },
            new SpamLevel
            {
                Id = 100,
                Name = "Check against the users IP address only",
                Description = "If the users IP address appears within the StopForumSpam database the user will be flagged as SPAM. Catches the most SPAM but can lead to more false positives.",
                Flags = RequestType.IpAddress
            }
        };

    }

}
